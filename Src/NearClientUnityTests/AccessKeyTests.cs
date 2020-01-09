
using NUnit.Framework;
using System.Threading.Tasks;
using NearClientUnity;
using NearClientUnityTests.Utils;
using NearClientUnity.Utilities;

namespace NearClientUnityTests
{
    [TestFixture]
    public class AccessKeyTests
    {

        private Near near;
        private Account testAccount;

        [OneTimeSetUp]
        protected async Task ClassInit()
        {
            near = await TestUtils.SetUpTestConnection();
            var masterAccount = await near.AccountAsync(accountId: TestUtils.TestAccountName);
            var amount = TestUtils.InitialBalance * new UInt128(100);
            testAccount = await TestUtils.CreateAccount(masterAccount: masterAccount, amount: amount);
        }

        [SetUp]
        protected async Task SetupBeforeEachTest()
        {
            
        }

        [Test]
        public void ShouldMakeFunctionCallUsingAccessKey()
        {

        }
    }
}
