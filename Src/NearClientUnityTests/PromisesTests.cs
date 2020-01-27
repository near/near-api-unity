using System;
using System.Dynamic;
using System.Threading.Tasks;
using NearClientUnity;
using NearClientUnity.Utilities;
using NearClientUnityTests.Utils;
using NUnit.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace NearClientUnityTests
{

    public class Result: IEquatable<Result>
    {
        public string n;
        public RSResult[] rs;

        public Result(string n, RSResult[] rs)
        {
            this.n = n;
            this.rs = rs;
        }

        public bool Equals(Result other)
        {            
            return n.Equals(other.n) && rs.Count() == other.rs.Count();
        }
    }

    public class RSResult: IEquatable<RSResult>
    {
        public bool ok;
        public Result r;

        public RSResult(bool ok, Result r)
        {
            this.ok = ok;
            this.r = r;
        }


        public bool Equals(RSResult other)
        {
            return ok.Equals(other.ok) && r.Equals(other.r);
        }
    }

    [TestFixture]
    public class PromisesTests
    {
        private Near _near;
        private Account _workingAccount;
        private string _contractName;
        private string _contractName1;
        private string _contractName2;        
        private ContractNear _contract;
        private ContractNear _contract1;
        private ContractNear _contract2;

        public void ClassInit()
        {
            ClassInitAsync().Wait();
        }

        [OneTimeSetUp]
        public async Task ClassInitAsync()
        {
            _near = await TestUtils.SetUpTestConnection();
            var masterAccount = await _near.AccountAsync(accountId: TestUtils.TestAccountName);
            var amount = TestUtils.InitialBalance * new UInt128(100);
            _workingAccount = await TestUtils.CreateAccount(masterAccount, amount);
        }

        public void SetupBeforeEachTest()
        {
            SetupBeforeEachTestAsync().Wait();
        }

        [SetUp]
        public async Task SetupBeforeEachTestAsync()
        {
            _contractName = TestUtils.GenerateUniqueString("cnt");
            _contractName1 = TestUtils.GenerateUniqueString("cnt");
            _contractName2 = TestUtils.GenerateUniqueString("cnt");
            _contract = await TestUtils.DeployContract(_workingAccount, _contractName, new UInt128(10000000));
            _contract1 = await TestUtils.DeployContract(_workingAccount, _contractName1, new UInt128(10000000));
            _contract2 = await TestUtils.DeployContract(_workingAccount, _contractName2, new UInt128(10000000));
        }

        //it should pass test single promise, no callback (A->B)
        [Test]
        public async Task ShouldPassTestSinglePromiseNoCallback()
        {
            Console.WriteLine("!!!!!" + (_contractName == _contractName1 && _contractName1 == _contractName2).ToString());
            dynamic changeParam = new ExpandoObject();
            changeParam.receiver = _contractName1;
            changeParam.methodName = "callbackWithName";
            changeParam.gas = 300000;
            changeParam.balance = 0;
            changeParam.callbackBalance = 0;
            changeParam.callbackGas = 0;
            dynamic args = new ExpandoObject();
            args.args = changeParam;
            var rawRealResult = await _contract.Change("callPromise", args, null, new UInt128(0));
            var realResult = rawRealResult.ToObject<Result>();
            Console.WriteLine("realResult" + realResult);
            dynamic viewParam = new ExpandoObject();
            string rawLastResult = (await _contract1.View("getLastResult", viewParam)).result;
            var lastResult = JsonConvert.DeserializeObject<Result>(rawLastResult);
            Assert.AreEqual(lastResult, new Result(n: _contractName1, rs: new RSResult[0])) ;
            Assert.AreEqual(realResult, lastResult);
        }

        //it should pass test single promise with callback (A->B=>A)
        [Test]
        public async Task ShouldPassTestSinglePromiseWithCallback()
        {
            dynamic changeParam = new ExpandoObject();
            changeParam.receiver = _contractName1;
            changeParam.methodName = "callbackWithName";
            changeParam.gas = 300000;
            changeParam.balance = 0;
            changeParam.callback = "callbackWithName";
            changeParam.callbackBalance = 0;
            changeParam.callbackGas = 200000;
            dynamic args = new ExpandoObject();
            args.args = changeParam;
            var rawRealResult = await _contract.Change("callPromise", args, null, new UInt128(0));
            var realResult = rawRealResult.ToObject<Result>();            
            dynamic viewParam = new ExpandoObject();
            string rawLastResult1 = (await _contract1.View("getLastResult", viewParam)).result;
            var lastResult1 = JsonConvert.DeserializeObject<Result>(rawLastResult1);
            Assert.AreEqual(lastResult1, new Result(n: _contractName1, rs: new RSResult[0]));            
            string rawLastResult = (await _contract.View("getLastResult", viewParam)).result;
            var lastResult = JsonConvert.DeserializeObject<Result>(rawLastResult);
            Console.WriteLine("lastResult" + lastResult);
            Assert.AreEqual(lastResult, new Result(n: _contractName, rs: new RSResult[1] { new RSResult(ok: true, r: lastResult1) }));
            Assert.AreEqual(realResult, lastResult);
        }
    }
}
