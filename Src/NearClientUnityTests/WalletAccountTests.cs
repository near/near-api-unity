using NearClientUnity;
using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NearClientUnityTests
{
    public class MockAuthService : IExternalAuthService
    {
        private readonly List<string> _urls = new List<string>();
        public List<string> Urls => _urls;

        public bool OpenUrl(string url)
        {
            _urls.Add(url);
            return true;
        }
    }

    [TestFixture]
    public class WalletAccountTests
    {
        private const string _walletUrl = "http://example.com/wallet";
        private MockAuthService _authService;        
        private KeyStore _keyStore;
        private Near _nearFake;
        private WalletAccount _walletAccount;

        [Test]
        public async Task CanCompleteSignIn()
        {
            var keyPair = KeyPair.FromRandom("ed25519");
            await _keyStore.SetKeyAsync("networkId", "pending_key" + keyPair.GetPublicKey().ToString(), keyPair);
            var url = "http://example.com/location?account_id=near.account&public_key=" + keyPair.GetPublicKey().ToString();
            await _walletAccount.CompleteSignIn(url);
            Assert.AreEqual((await _keyStore.GetKeyAsync("networkId", "near.account")).ToString(), keyPair.ToString());
            Assert.IsTrue(_walletAccount.IsSignedIn());
            Assert.AreEqual("near.account", _walletAccount.GetAccountId());
        }

        [Test]
        public async Task CanRequestSignIn()
        {
            await _walletAccount.RequestSignIn("signInContract", "signInTitle", new Uri("http://example.com/success"), new Uri("http://example.com/fail"), new Uri("http://example.com/location"));
            var accounts = await _keyStore.GetAccountsAsync("networkId");
            Assert.AreEqual(1, accounts.Length);
            Assert.IsTrue(accounts[0].Contains("pending_key"));

            var expected = new UriBuilder("http://example.com/");
            var keyPair = await _keyStore.GetKeyAsync("networkId", accounts[0]);
            var publicKey = keyPair.GetPublicKey().ToString();
            expected.Query = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "title", "signInTitle" },
                { "contract_id", "signInContract" },
                { "success_url", "http://example.com/success" },
                { "failure_url", "http://example.com/fail" },
                { "app_url", "http://example.com/location"},
                { "public_key", publicKey},
            }).ReadAsStringAsync().Result;

            var actual = new Uri(_authService.Urls[0]);

            Assert.AreEqual(expected.Uri.Query, actual.Query);
            Assert.AreEqual(expected.Uri.Host, actual.Host);
        }

        //not signed in by default
        [Test]
        public void NotSignedInByDefault()
        {
            Assert.IsFalse(_walletAccount.IsSignedIn());
        }

        [TearDown]
        public void SetupAfterEachTestAsync()
        {
            _walletAccount.NearLocalStorage.Settings.Clear();
        }

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
    }
}