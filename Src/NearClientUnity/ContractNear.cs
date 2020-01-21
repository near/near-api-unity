using NearClientUnity.Providers;
using NearClientUnity.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public class ContractNear
    {
        private readonly Account _account;
        private dynamic _changeMethods;
        private readonly string _contractId;
        private dynamic _viewMethods;

        public ContractNear(Account account, string contractId, ContractOptions options)
        {            
            _account = account;
            _contractId = contractId;
            var dynamicViewMethods = new ExpandoObject() as IDictionary<string, Object>;
            foreach (var vm in options.viewMethods)
            {
                Console.WriteLine("viewMethods " + vm);
                var method = new Func<dynamic, Task<dynamic>>(async (dynamic args) =>
                {
                    var result = await _account.ViewFunctionAsync(_contractId, vm, args);
                    return result;
                });
                dynamicViewMethods.Add(vm, method);
            }
            _viewMethods = dynamicViewMethods;

            var dynamicChangeMethods = new ExpandoObject() as IDictionary<string, Object>;
            foreach (var cm in options.changeMethods)
            {
                Console.WriteLine("changeMethods " + cm);
                var method = new Func<dynamic, ulong, UInt128, Task<dynamic>>(
                    async (dynamic args, ulong gas, UInt128 amount) =>
                    {
                        var rawResult = await _account.FunctionCallAsync(_contractId, cm, args, gas, amount);                        
                        var result = Provider.GetTransactionLastResult(rawResult);
                        return result;
                    });
                dynamicChangeMethods.Add(cm, method);
            }
            _changeMethods = dynamicChangeMethods;
        }

        public async Task<dynamic> Change(string methodName, dynamic args, ulong? gas, UInt128 amount)
        {
            var rawResult = await _account.FunctionCallAsync(_contractId, methodName, args, gas, amount);
            return Provider.GetTransactionLastResult(rawResult);
        }

        public async Task<dynamic> View(string methodName, dynamic args)
        {
            var rawResult = await _account.ViewFunctionAsync(_contractId, methodName, args);
            dynamic data = new ExpandoObject();
            var logs = new List<string>();
            var result = new List<byte>();
            foreach (var log in rawResult.logs)
            {
                logs.Add((string)log);
            }
            foreach (var item in rawResult.result)
            {
                result.Add((byte)item);
            }
            data.logs = logs.ToArray();
            data.result = Encoding.UTF8.GetString(result.ToArray()).Trim('"');
            return data;
        }
    }
}