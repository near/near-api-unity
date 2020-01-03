using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NearClientUnityTests.KeyStores
{
    [TestFixture]
    public class MergeKeyStoreTests : KeyStoreTests
    {
        [OneTimeSetUp]
        public void ClassInit()
        {
            _keyStore = new MergeKeyStore(new KeyStore[] {new InMemoryKeyStore(), new InMemoryKeyStore()});
        }

        [SetUp]
        public void SetupBeforeEachTest()
        {
            SetupBeforeEachTestAync().Wait();
        }

        [Test]
        public async Task GetAllAccountsWithEmptyNetworkReturnsEmptyArray()
        {
            await GetAllAccountsWithEmptyNetworkReturnsEmptyArray_BaseTest();
        }

        [Test]
        public async Task GetAllAccountsWithSingleNetworkInKeystoreReturnsArrayIds()
        {
            await GetAllAccountsWithSingleNetworkInKeystoreReturnsArrayIds_BaseTest();
        }

        [Test]
        public async Task GetKeyPairForNotExistingAccountReturnsNull()
        {
            await GetKeyPairForNotExistingAccountReturnsNull_BaseTest();
        }

        [Test]
        public async Task GetKeyPairFromNetworkWithAccountInKeystoreReturnsKeyPair()
        {
            await GetKeyPairFromNetworkWithAccountInKeystoreReturnsKeyPair_BaseTest();
        }

        [Test]
        public async Task GetNetworksInKeystoreReturnsArrayNetworks()
        {
            await GetNetworksInKeystoreReturnsArrayNetworks_BaseTest();
        }

        [Test]
        public async Task AddTwoKeysToNetworkAndRetrieveThem()
        {
            await AddTwoKeysToNetworkAndRetrieveThem_BaseTest();
        }

        [Test]
        public async Task LooksUpKeyFromFallbackKeyStore()
        {
            var expectedKey = KeyPair.FromRandom("ED25519");
            await _keyStore.Stores[1].SetKeyAsync("network", "account", expectedKey);
            var actualKey = await _keyStore.GetKeyAsync("network", "account");
            Assert.AreEqual(expectedKey.ToString(), actualKey.ToString());
        }

        [Test]
        public async Task LooksUpKeyInProperOrder()
        {
            var expectedKey1 = KeyPair.FromRandom("ED25519");
            var expectedKey2 = KeyPair.FromRandom("ED25519");
            await _keyStore.Stores[0].SetKeyAsync("network", "account", expectedKey1);
            await _keyStore.Stores[1].SetKeyAsync("network", "account", expectedKey2);
            var actualKey1 = await _keyStore.GetKeyAsync("network", "account");
            Assert.AreEqual(expectedKey1.ToString(), actualKey1.ToString());
        }

        [Test]
        public async Task SetsKeysOnlyInFirstKeyStore()
        {
            var key1 = KeyPair.FromRandom("ED25519");
            await _keyStore.SetKeyAsync("network", "account", key1);
            Assert.AreEqual(1,(await _keyStore.Stores[0].GetAccountsAsync("network")).Length);
            Assert.AreEqual(0, (await _keyStore.Stores[1].GetAccountsAsync("network")).Length);

        }
    }
}