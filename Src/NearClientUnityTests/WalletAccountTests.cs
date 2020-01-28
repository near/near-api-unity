using System;
using System.Threading.Tasks;
using NearClientUnity;
using NearClientUnity.KeyStores;
using NUnit.Framework;

namespace NearClientUnityTests
{
    [TestFixture]
    public class WalletAccountTests
    {

        //private WalletAccount _walletAccount;
        private KeyStore _keyStore;
        private const string _walletUrl = "http://example.com/wallet";
        private Near _nearFake;
        private string _contractName2;       

        public void SetupBeforeEachTest()
        {
            SetupBeforeEachTestAsync().Wait();
        }

        [SetUp]
        public async Task SetupBeforeEachTestAsync()
        {
            _keyStore = new InMemoryKeyStore();
            _nearFake = new Near(config: new NearConfig()
            {
                NetworkId = "networkId",
                NodeUrl = _walletUrl,
                ProviderType = ProviderType.JsonRpc,
                SignerType = SignerType.InMemory,
                KeyStore = _keyStore,
                ContractName = "contractId",
                WalletUrl = _walletUrl
            });            
        }

        public void SetupAfterEachTest()
        {
            SetupAfterEachTestAsync().Wait();
        }

        [TearDown]
        public async Task SetupAfterEachTestAsync()
        {

        }
    }
}
