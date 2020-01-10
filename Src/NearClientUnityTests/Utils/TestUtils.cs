using NearClientUnity;
using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using System;
using System.Threading.Tasks;

namespace NearClientUnityTests.Utils
{
    public class TestUtils
    {
        public static readonly string TestAccountName = "test.near";
        public static readonly string NetworkId = "unittest";
        public static readonly UInt128 InitialBalance = 100000000000;

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

        public static async Task<Account> CreateAccount(Account masterAccount, UInt128 amount, uint trial = 5)
        {
            await masterAccount.FetchStateAsync();
            var newAccountName = GenerateUniqueString(prefix: "text");
            var newPublicKey = await masterAccount.Connection.Signer.CreateKeyAsync(newAccountName, TestUtils.NetworkId);
            await masterAccount.CreateAccountAsync(newAccountName, newPublicKey, amount);
            return new Account(masterAccount.Connection, newAccountName);
        }

        private static string GenerateUniqueString(string prefix)
        {
            var timestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            var randomInt = new Random().Next(0, 1000);
            return prefix + timestamp + randomInt;
        }
    }
}
