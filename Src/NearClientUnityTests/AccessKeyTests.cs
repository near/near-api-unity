
using NUnit.Framework;
using System.Threading.Tasks;
using NearClientUnity;
using NearClientUnityTests.Utils;
using NearClientUnity.Utilities;
using System.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NearClientUnityTests
{
    [TestFixture]
    public class AccessKeyTests
    {

        private Near _near;
        private Account _testAccount;
        private Account _workingAccount;
        private string _contractId;
        private ContractNear _contract;

        
        public void ClassInit()
        {
            ClassInitAsync().Wait();
        }

        [OneTimeSetUp]
        public async Task ClassInitAsync()
        {
            Console.WriteLine("Start OneTimeSetUp");
            _near = await TestUtils.SetUpTestConnection();
            Console.WriteLine("Connection == null -> " + (_near.Connection == null));
            var masterAccount = await _near.AccountAsync(accountId: TestUtils.TestAccountName);
            var amount = TestUtils.InitialBalance * new UInt128(100);
            _testAccount = await TestUtils.CreateAccount(masterAccount: masterAccount, amount: amount);
            Console.WriteLine("End OneTimeSetUp");            
        }

        
        public void SetupBeforeEachTest()
        {
            SetupBeforeEachTestAsync().Wait();
        }

        [SetUp]
        public async Task SetupBeforeEachTestAsync()
        {
            Console.WriteLine("Start SetUp");
            _contractId = TestUtils.GenerateUniqueString(prefix: "test");
            _workingAccount = await TestUtils.CreateAccount(masterAccount: _testAccount, amount: TestUtils.InitialBalance);
            _contract = await TestUtils.DeployContract(_workingAccount, _contractId, new UInt128(10000000));
            Console.WriteLine("End SetUp");
        }

        [Test]
        public async Task ShouldMakeFunctionCallUsingAccessKey()
        {
            var keypair = KeyPairEd25519.FromRandom();
            var publicKey = keypair.GetPublicKey();
            await _workingAccount.AddKeyAsync(publicKey.ToString(), new UInt128(10000000), "", _contractId);
            var signer = (InMemorySigner)this._near.Connection.Signer;
            await signer.KeyStore.SetKeyAsync(TestUtils.NetworkId, _workingAccount.AccountId, keypair);
            var setCallValue = TestUtils.GenerateUniqueString(prefix: "setCallPrefix");            
            dynamic args = new ExpandoObject();
            args.value = setCallValue;            
            await _contract.Change("setValue", args, null, new UInt128(0));
            var viewArgs = new ExpandoObject();
            var testValue = await _contract.View("getValue", viewArgs);                        
            Assert.AreEqual(testValue.result, setCallValue);
        }

        [Test]
        public async Task ShouldRemoveAccessKeyNoLongerWorks()
        {
            var keypair = KeyPairEd25519.FromRandom();
            var publicKey = keypair.GetPublicKey();
            await _workingAccount.AddKeyAsync(publicKey.ToString(), new UInt128(400000), "", _contractId);
            await _workingAccount.DeleteKeyAsync(publicKey: publicKey.ToString());
            var signer = (InMemorySigner)this._near.Connection.Signer;
            await signer.KeyStore.SetKeyAsync(TestUtils.NetworkId, _workingAccount.AccountId, keypair);
            dynamic args = new ExpandoObject();
            args.value = "test";
            try
            {
                var changeResult = await _contract.Change("setValue", args, null, new UInt128(0));
                Assert.Fail("should throw an error");
            }
            catch (Exception e)
            {
                Assert.Pass("pass with exception", e);
            }           
        }

        [Test]
        public async Task ShouldViewAccountDetailsAfterAddingAccessKeys()
        {
            var keypair = KeyPairEd25519.FromRandom();
            var publicKey = keypair.GetPublicKey();
            await _workingAccount.AddKeyAsync(publicKey.ToString(), new UInt128(1000000000), "", _contractId);
            var contractId2 = "test_contract2_" + (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var contract2 = await TestUtils.DeployContract(_workingAccount, contractId2, new UInt128(10000000));
            var keypair2 = KeyPairEd25519.FromRandom();
            var publicKey2 = keypair2.GetPublicKey();
            await _workingAccount.AddKeyAsync(publicKey2.ToString(), new UInt128(2000000000), "", contractId2);
            var details = await _workingAccount.GetAccountDetailsAsync();

            var expectedResult = new List<dynamic>();
            dynamic authorizedApp1 = new ExpandoObject();
            authorizedApp1.ContractId = _contractId;
            authorizedApp1.Amount = new UInt128(1000000000);
            authorizedApp1.PublicKey = publicKey.ToString();
            expectedResult.Add(authorizedApp1);

            dynamic authorizedApp2 = new ExpandoObject();
            authorizedApp2.ContractId = contractId2;
            authorizedApp2.Amount = new UInt128(2000000000);
            authorizedApp2.PublicKey = publicKey2.ToString();
            expectedResult.Add(authorizedApp2);

            IEnumerable<string> expected = expectedResult.Select(x => ((object)x.ContractId).ToString()).ToList().OrderBy(s => s);
            dynamic[] apps = details.AuthorizedApps;            
            IEnumerable<string> real = apps.Select(x => ((object)x.ContractId).ToString()).ToList().OrderBy(s => s);           
            Assert.IsTrue(real.SequenceEqual(expected));                       
        }

        [Test]
        public async Task ShouldLoadingAccountAfterAddingAFullKey()
        {
            var keypair = KeyPairEd25519.FromRandom();
            var publicKey = keypair.GetPublicKey();
            await _workingAccount.AddKeyAsync(publicKey.ToString(), null, "", "");
            var rawAccessKeys = await _workingAccount.GetAccessKeysAsync();
            var accessKeys = new List<dynamic>();
            foreach(dynamic accessKey in rawAccessKeys)
            {
                accessKeys.Add(accessKey);
            }
            Assert.That(accessKeys, Has.Exactly(2).Items);
            var addedKey = accessKeys.First(obj => ((string)obj.public_key).Equals(publicKey.ToString()));
            Console.WriteLine("addedKey " + addedKey);
            Assert.IsNotNull(addedKey);
            var permission = (string)addedKey.access_key.permission;
            Assert.AreEqual(permission, "FullAccess");
        }
    }
}
