
using NUnit.Framework;
using System.Threading.Tasks;
using NearClientUnity;
using NearClientUnityTests.Utils;
using NearClientUnity.Utilities;
using System.Collections.Generic;
using System.Dynamic;
using System;

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

        [OneTimeSetUp]
        public void ClassInit()
        {
            ClassInitAsync().Wait();
        }

        protected async Task ClassInitAsync()
        {
            _near = await TestUtils.SetUpTestConnection();
            Console.WriteLine("Connection == null -> " + (_near.Connection == null));
            var masterAccount = await _near.AccountAsync(accountId: TestUtils.TestAccountName);
            var amount = TestUtils.InitialBalance * new UInt128(100);
            _testAccount = await TestUtils.CreateAccount(masterAccount: masterAccount, amount: amount);
        }

        [SetUp]
        public void SetupBeforeEachTest()
        {
            SetupBeforeEachTestAsync().Wait();
        }

        protected async Task SetupBeforeEachTestAsync()
        {
            _contractId = TestUtils.GenerateUniqueString(prefix: "test");
            _workingAccount = await TestUtils.CreateAccount(masterAccount: _testAccount, amount: TestUtils.InitialBalance);
            _contract = await TestUtils.DeployContract(_workingAccount, _contractId, new UInt128(10000000));
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
            //var changeArgs = new
            //{
            //    value = setCallValue
            //};
            dynamic args = new ExpandoObject();
            args.value = setCallValue;
            //Dictionary<string, dynamic> changeArgs = new Dictionary<string, dynamic>();
            //changeArgs.Add("value", setCallValue);
            await _contract.Change("setValue", args, null, new UInt128(0));
            var viewArgs = new { };
            var testValue = await _contract.View("getValue", viewArgs);
            Assert.AreEqual(testValue, setCallValue);
        }
    }
}
