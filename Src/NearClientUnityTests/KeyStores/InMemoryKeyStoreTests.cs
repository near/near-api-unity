using NearClientUnity.KeyStores;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NearClientUnityTests.KeyStores
{
    [TestFixture]
    public class InMemoryKeyStoreTests : KeyStoreTests
    {
        [Test]
        public async Task AddTwoKeysToNetworkAndRetrieveThem()
        {
            await AddTwoKeysToNetworkAndRetrieveThem_BaseTest();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            _keyStore = new InMemoryKeyStore();
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

        [SetUp]
        public void SetupBeforeEachTest()
        {
            SetupBeforeEachTestAync().Wait();
        }
    }
}