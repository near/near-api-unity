using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

namespace NearClientUnityTests
{
    [TestFixtureSource(typeof(FixtureArgs))]
    public class KeyStoreTests
    {
        private const string NETWORK_ID_SINGLE_KEY = "singlekeynetworkid";
        private const string ACCOUNT_ID_SINGLE_KEY = "singlekey_accountid";
        private readonly KeyPair KEYPAIR_SINGLE_KEY = new KeyPairEd25519("2wyRcSwSuHtRVmkMCGjPwnzZmQLeXLzLLyED1NDMt4BjnKgQL6tF85yBx6Jr26D2dUNeC716RBoTxntVHsegogYw");

        private KeyStore _keyStore;

        public KeyStoreTests(KeyStore keyStore)
        {
            _keyStore = keyStore;
        }

        public async Task SetupBeforeEachTestAync()
        {
            await _keyStore.ClearAsync();
            await _keyStore.SetKeyAsync(NETWORK_ID_SINGLE_KEY, ACCOUNT_ID_SINGLE_KEY, KEYPAIR_SINGLE_KEY);
        }


        [SetUp]
        public void SetupBeforeEachTest()
        {
            SetupBeforeEachTestAync().Wait();
        }


        [Test]
        public async Task GetAllAccountsWithEmptyNetworkReturnsEmptyList()
        {
            var actualList = await _keyStore.GetAccountsAsync("emptynetwork");
            var expectedList = new string[0];
            Assert.AreEqual(expectedList, actualList);
        }
    }

    class FixtureArgs : IEnumerable
    {
        private readonly string KEYSTORE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "test-keys");
        public IEnumerator GetEnumerator()
        {
            yield return new InMemoryKeyStore();
            yield return new UnencryptedFileSystemKeyStore(KEYSTORE_PATH);
        }
    }
}