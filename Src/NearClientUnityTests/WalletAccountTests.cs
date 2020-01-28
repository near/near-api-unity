using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NearClientUnity;
using NearClientUnity.KeyStores;
using NUnit.Framework;

namespace NearClientUnityTests
{

    public class MockAuthService : IExternalAuthService
    {
        private List<string> urls;

        public bool OpenUrl(string url)
        {
            urls.Add(url);
            return true;
        }
    }

    [TestFixture]
    public class WalletAccountTests
    {

        private WalletAccount _walletAccount;
        private KeyStore _keyStore;
        private const string _walletUrl = "http://example.com/wallet";
        private Near _nearFake;
        private string _contractName2;
        private IExternalAuthService _authService;

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
            _authService = new MockAuthService();
            _walletAccount = new WalletAccount(_nearFake, "", _authService);
        }
               
        [TearDown]
        public void SetupAfterEachTestAsync()
        {
            //_walletAccount._nearLocalStorage.Settings.Clear();
        }

        //not signed in by default
        [Test]
        public void NotSignedInByDefault()
        {
            Assert.IsFalse(_walletAccount.IsSignedIn());
        }
    }
}
