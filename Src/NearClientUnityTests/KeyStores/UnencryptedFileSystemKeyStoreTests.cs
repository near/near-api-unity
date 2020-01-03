using NearClientUnity.KeyStores;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NearClientUnityTests.KeyStores
{
    public class UnencryptedFileSystemKeyStoreTests : KeyStoreTests
    {
        private readonly string _keystorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "test-keys");
        [OneTimeSetUp]
        public void ClassInit()
        {
            _keyStore = new UnencryptedFileSystemKeyStore(_keystorePath);
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
    }
}