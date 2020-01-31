using NearClientUnity;
using NearClientUnity.Providers;
using NearClientUnity.Utilities;
using NearClientUnityTests.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("NearClientUnity")]

namespace NearClientUnityTests
{
    public class TestConsoleWriter : TextWriter
    {
        private readonly List<string> _logs;
        private StringBuilder content = new StringBuilder();

        public TestConsoleWriter(List<string> logs)
        {
            _logs = logs;
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        public override void Write(char value)
        {
            base.Write(value);
            content.Append(value);
            if (value == '\n')
            {
                // for Windows
                var result = content.ToString().Replace("\r\n", string.Empty);
                // for other OS
                result = result.Replace("\n", string.Empty);
                _logs.Add(result);
                content = new StringBuilder();
            }
        }
    }

    [TestFixture]
    internal class AccountWithDeployContractTests
    {
        private dynamic _contract;
        private string _contractId;
        private List<string> _logs;
        private Near _near;
        private Account _workingAccount;
        private TextWriter defaultConsoleOut;

        [TearDown]
        public void AfterEachTest()
        {
            Console.SetOut(defaultConsoleOut);
            defaultConsoleOut = null;
            _logs = null;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            defaultConsoleOut = Console.Out;
            _logs = new List<string>();
            Console.SetOut(new TestConsoleWriter(_logs));
        }

        [Test]
        public void CanGetAssertMessageFromMethodResult()
        {
            Assert.That(async () => await _contract.triggerAssert(), Throws.TypeOf<Exception>());
            Assert.AreEqual($"[{_contractId}]: LOG: log before assert", _logs[0]);
            Assert.IsTrue(_logs[1].Contains($"[{_contractId}]: ABORT: expected to fail, filename: \"assembly/main.ts\""));
        }

        [Test]
        public async Task CanGetLogsFromMethodResult()
        {
            string[] expectedResult = new[] { $"[{_contractId}]: LOG: log1", $"[{_contractId}]: LOG: log2" };
            await _contract.generateLogs();
            string[] actualResult = _logs.ToArray();
            Assert.AreEqual(expectedResult, actualResult);
        }

        public void ClassInit()
        {
            ClassInitAsync().Wait();
        }

        [OneTimeSetUp]
        public async Task ClassInitAsync()
        {
            _contractId = TestUtils.GenerateUniqueString("test_contract");
            _near = await TestUtils.SetUpTestConnection();
            _workingAccount = await TestUtils.CreateAccount(await _near.AccountAsync(TestUtils.TestAccountName), TestUtils.InitialBalance * (UInt128)100);

            var newPublicKey = await _near.Connection.Signer.CreateKeyAsync(_contractId, TestUtils.NetworkId);

            var data = Wasm.GetBytes();

            await _workingAccount.CreateAndDeployContractAsync(_contractId, newPublicKey, data, (UInt128)1000000);

            var contractOptions = new ContractOptions()
            {
                viewMethods = new string[] { "hello", "getValue", "getAllKeys", "returnHiWithLogs" },
                changeMethods = new string[] { "setValue", "generateLogs", "triggerAssert", "testSetRemove" },
            };

            _contract = new ContractNear(_workingAccount, _contractId, contractOptions);
        }

        [Test]
        public async Task MakeFunctionCallsViaAccount()
        {
            var expectedResult = "hello trex";
            dynamic args = new ExpandoObject();
            args.name = "trex";
            var rawJsonObject = await _workingAccount.ViewFunctionAsync(_contractId, "hello", args);

            var result = new List<byte>();

            foreach (var item in rawJsonObject.result)
            {
                result.Add((byte)item);
            }

            var actualResult = Encoding.UTF8.GetString(result.ToArray()).Trim('"');

            Assert.AreEqual(expectedResult, actualResult);

            var setCallValue = TestUtils.GenerateUniqueString("setCallPrefix");
            expectedResult = setCallValue;

            args = new ExpandoObject();
            args.value = setCallValue;

            actualResult = Provider.GetTransactionLastResult(await _workingAccount.FunctionCallAsync(_contractId, "setValue", args));

            Assert.AreEqual(expectedResult, actualResult);

            args = new ExpandoObject();
            rawJsonObject = await _workingAccount.ViewFunctionAsync(_contractId, "getValue", args);
            result = new List<byte>();

            foreach (var item in rawJsonObject.result)
            {
                result.Add((byte)item);
            }

            actualResult = Encoding.UTF8.GetString(result.ToArray()).Trim('"');

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task MakeFunctionCallsViaContract()
        {
            var expectedResult = "hello trex";

            dynamic args = new ExpandoObject();
            args.name = "trex";

            var rawJsonObject = await _contract.hello(args);

            var result = new List<byte>();

            foreach (var item in rawJsonObject.result)
            {
                result.Add((byte)item);
            }

            var actualResult = Encoding.UTF8.GetString(result.ToArray()).Trim('"');

            Assert.AreEqual(expectedResult, actualResult);

            var setCallValue = TestUtils.GenerateUniqueString("setCallPrefix");
            expectedResult = setCallValue;

            args = new ExpandoObject();
            args.value = setCallValue;

            actualResult = await _contract.setValue(args);

            Assert.AreEqual(expectedResult, actualResult);

            rawJsonObject = await _contract.getValue();

            result = new List<byte>();

            foreach (var item in rawJsonObject.result)
            {
                result.Add((byte)item);
            }

            actualResult = Encoding.UTF8.GetString(result.ToArray()).Trim('"');

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task MakeFunctionCallsViaContractWithGas()
        {
            ulong gas = 100000;
            var expectedResult = "hello trex";

            dynamic args = new ExpandoObject();
            args.name = "trex";

            var rawJsonObject = await _contract.hello(args);

            var result = new List<byte>();

            foreach (var item in rawJsonObject.result)
            {
                result.Add((byte)item);
            }

            var actualResult = Encoding.UTF8.GetString(result.ToArray()).Trim('"');

            Assert.AreEqual(expectedResult, actualResult);

            var setCallValue = TestUtils.GenerateUniqueString("setCallPrefix");
            expectedResult = setCallValue;

            args = new ExpandoObject();
            args.value = setCallValue;

            actualResult = await _contract.setValue(args, gas);

            Assert.AreEqual(expectedResult, actualResult);

            rawJsonObject = await _contract.getValue();

            result = new List<byte>();

            foreach (var item in rawJsonObject.result)
            {
                result.Add((byte)item);
            }

            actualResult = Encoding.UTF8.GetString(result.ToArray()).Trim('"');

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task TestSetRemove()
        {
            dynamic args = new ExpandoObject();
            args.value = "123";

            bool? result = null;

            try
            {
                await _contract.testSetRemove(args);
                result = true;
            }
            catch
            {
                result = false;
            }

            Assert.IsTrue(result);
        }
    }
}