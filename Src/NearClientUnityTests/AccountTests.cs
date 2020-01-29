using NearClientUnity;
using NearClientUnity.Utilities;
using NearClientUnityTests.Utils;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace NearClientUnityTests
{
    [TestFixture]
    internal class AccountTests
    {
        private Near _near;
        private Account _workingAccount;

        [OneTimeSetUp]
        public async Task ClassInitAsync()
        {
            _near = await TestUtils.SetUpTestConnection();
            _workingAccount = await TestUtils.CreateAccount(await _near.AccountAsync(TestUtils.TestAccountName), TestUtils.InitialBalance * (UInt128)100);
        }

        [Test]
        public async Task CreateAccountAndThenViewAccountReturnsTheCreatedAccount()
        {
            var newAccountName = TestUtils.GenerateUniqueString("test");
            var newAccountPublicKey = "9AhWenZ3JddamBoyMqnTbp7yVbRuvqAv3zwfrWgfVRJE";
            await _workingAccount.CreateAccountAsync(newAccountName, newAccountPublicKey, TestUtils.InitialBalance);
            var newAccount = new Account(_near.Connection, newAccountName);
            var state = await newAccount.GetStateAsync();
            var expectedResult = TestUtils.InitialBalance.ToString();
            var actualResult = state.Amount;
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void CreateExistingAccount()
        {
            Assert.That(async () => await _workingAccount.CreateAccountAsync(_workingAccount.AccountId, "9AhWenZ3JddamBoyMqnTbp7yVbRuvqAv3zwfrWgfVRJE", 100), Throws.TypeOf<Exception>());
        }

        [Test]
        public async Task DeleteAccount()
        {
            var sender = await TestUtils.CreateAccount(_workingAccount, TestUtils.InitialBalance);
            var receiver = await TestUtils.CreateAccount(_workingAccount, TestUtils.InitialBalance);
            await sender.DeleteAccountAsync(receiver.AccountId);
            var reloaded = new Account(sender.Connection, sender.AccountId);
            Assert.That(async () => await reloaded.GetStateAsync(), Throws.TypeOf<Exception>());
        }

        [Test]
        public async Task SendMoney()
        {
            var sender = await TestUtils.CreateAccount(_workingAccount, TestUtils.InitialBalance);
            var receiver = await TestUtils.CreateAccount(_workingAccount, TestUtils.InitialBalance);
            await sender.SendMoneyAsync(receiver.AccountId, (UInt128)10000);
            await receiver.FetchStateAsync();
            var state = await receiver.GetStateAsync();
            var expectedResult = (TestUtils.InitialBalance + (UInt128)10000).ToString();
            var actualResult = state.Amount;
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task ViewPreDefinedAccountWorksAndReturnsCorrectName()
        {
            var expectedResult = "11111111111111111111111111111111";
            var rawResult = await _workingAccount.GetStateAsync();
            var actualResult = rawResult.CodeHash;
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}