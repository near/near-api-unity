using NearClientUnity;
using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NearClientUnityTests.Utils
{
    public class TestUtils
    {
        public static readonly UInt128 InitialBalance = 100000000000;
        public static readonly string NetworkId = "unittest";
        public static readonly string TestAccountName = "test.near";

        public static async Task<Account> CreateAccount(Account masterAccount, UInt128 amount, uint trial = 5)
        {
            await masterAccount.FetchStateAsync();
            var newAccountName = GenerateUniqueString(prefix: "test");
            var newPublicKey = await masterAccount.Connection.Signer.CreateKeyAsync(newAccountName, TestUtils.NetworkId);
            await masterAccount.CreateAccountAsync(newAccountName, newPublicKey, amount);
            return new Account(masterAccount.Connection, newAccountName);
        }

        public static async Task<ContractNear> DeployContract(Account workingAccount, string contractId, UInt128 amount)
        {
            var newPublicKey = await workingAccount.Connection.Signer.CreateKeyAsync(contractId, TestUtils.NetworkId);
            var wasmBytes = Wasm.GetBytes();
            await workingAccount.CreateAndDeployContractAsync(contractId, newPublicKey, wasmBytes, amount);
            var options = new ContractOptions()
            {
                viewMethods = new string[] { "getValue", "getLastResult" },
                changeMethods = new string[] { "setValue", "callPromise" }
            };
            return new ContractNear(workingAccount, contractId, options);
        }

        public static string GenerateUniqueString(string prefix)
        {
            int length = 20;
            const string valid = "1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return $"{prefix}{res.ToString()}";
        }

        public static async Task<Near> SetUpTestConnection()
        {
            var keyStore = new InMemoryKeyStore();
            var key = "ed25519:2wyRcSwSuHtRVmkMCGjPwnzZmQLeXLzLLyED1NDMt4BjnKgQL6tF85yBx6Jr26D2dUNeC716RBoTxntVHsegogYw";
            var keyPair = KeyPair.FromString(key);
            await keyStore.SetKeyAsync(NetworkId, TestAccountName, keyPair);
            var environment = EnvironmentConfig.GetConfig(Environment.CI);
            var config = new NearConfig()
            {
                NetworkId = TestUtils.NetworkId,
                NodeUrl = environment.NodeUrl.AbsoluteUri,
                ProviderType = ProviderType.JsonRpc,
                SignerType = SignerType.InMemory,
                KeyStore = keyStore,
                ContractName = "contractId",
                WalletUrl = environment.NodeUrl.AbsoluteUri
            };
            return await Near.ConnectAsync(config: config);
        }
    }
}