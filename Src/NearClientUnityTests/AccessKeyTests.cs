
using NUnit.Framework;
using System.Threading.Tasks;
using NearClientUnity;
using NearClientUnityTests.Utils;
using NearClientUnity.Utilities;
using System.Collections.Generic;
using System.Dynamic;
using System;
using System.IO;
using System.Text;

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
            var viewArgs = new ExpandoObject(); ;
            var testValue = await _contract.View("getValue", viewArgs);                        
            Assert.AreEqual(testValue.result, setCallValue);
        }
    }
}
