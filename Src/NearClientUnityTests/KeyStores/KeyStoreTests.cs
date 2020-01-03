
using System.Threading.Tasks;
using NearClientUnity.Utilities;
using NUnit.Framework;

namespace NearClientUnityTests.KeyStores
{
    public class KeyStoreTests
    {
        private const string NetworkIdSingleKey = "singlekeynetworkid";
        private const string AccountIdSingleKey = "singlekey_accountid";
        private readonly KeyPair _keypairSingleKey = new KeyPairEd25519("2wyRcSwSuHtRVmkMCGjPwnzZmQLeXLzLLyED1NDMt4BjnKgQL6tF85yBx6Jr26D2dUNeC716RBoTxntVHsegogYw");

        protected dynamic _keyStore;

        protected async Task SetupBeforeEachTestAync()
        {
            await _keyStore.ClearAsync();
            await _keyStore.SetKeyAsync(NetworkIdSingleKey, AccountIdSingleKey, _keypairSingleKey);
        }
        
        protected async Task GetAllAccountsWithEmptyNetworkReturnsEmptyArray_BaseTest()
        {
            var actualArray = await _keyStore.GetAccountsAsync("emptynetwork");
            var expectedArray = new string[0];
            Assert.AreEqual(expectedArray, actualArray);
        }
        
        protected async Task GetAllAccountsWithSingleNetworkInKeystoreReturnsArrayIds_BaseTest()
        {
            var actualArray = await _keyStore.GetAccountsAsync(NetworkIdSingleKey);
            var expectedArray = new[] { AccountIdSingleKey };
            Assert.AreEqual(expectedArray, actualArray);
        }
        
        protected async Task GetKeyPairForNotExistingAccountReturnsNull_BaseTest()
        {
            KeyPair actualKeyPair = await _keyStore.GetKeyAsync("somenetwork", "someaccount");
            KeyPair expectedKeyPair = null;
            Assert.AreEqual(expectedKeyPair, actualKeyPair);
        }
        
        protected async Task GetKeyPairFromNetworkWithAccountInKeystoreReturnsKeyPair_BaseTest()
        {
            var actualKeyPair = await _keyStore.GetKeyAsync(NetworkIdSingleKey, AccountIdSingleKey);
            var expectedKeyPair = _keypairSingleKey;
            Assert.AreEqual(expectedKeyPair.ToString(), actualKeyPair.ToString());
        }
        
        protected async Task GetNetworksInKeystoreReturnsArrayNetworks_BaseTest()
        {
            var actualArray = await _keyStore.GetNetworksAsync();
            var expectedArray = new[] { NetworkIdSingleKey };
            Assert.AreEqual(expectedArray, actualArray);
        }

        protected async Task AddTwoKeysToNetworkAndRetrieveThem_BaseTest()
        {
            const string networkId = "twoKeyNetwork";
            const string accountId1 = "acc1";
            const string accountId2 = "acc2";
            var expectedKey1 = KeyPair.FromRandom("ED25519");
            var expectedKey2 = KeyPair.FromRandom("ED25519");
            await _keyStore.SetKeyAsync(networkId, accountId1, expectedKey1);
            await _keyStore.SetKeyAsync(networkId, accountId2, expectedKey2);
            var actualKey1 = await _keyStore.GetKeyAsync(networkId, accountId1);
            var actualKey2 = await _keyStore.GetKeyAsync(networkId, accountId2);
            Assert.AreEqual(expectedKey1.ToString(), actualKey1.ToString());
            Assert.AreEqual(expectedKey2.ToString(), actualKey2.ToString());

            var actualAccountIds = await _keyStore.GetAccountsAsync(networkId);
            var expectedAccountIds = new[] { accountId1, accountId2 };
            Assert.AreEqual(expectedAccountIds, actualAccountIds);

            var actualNetworks = await _keyStore.GetNetworksAsync();
            var expectedNetworks = new[] { NetworkIdSingleKey, networkId };
            Assert.AreEqual(expectedNetworks, actualNetworks);
        }
    }
}