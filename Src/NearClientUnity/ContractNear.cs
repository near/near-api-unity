using NearClientUnity.Providers;
using NearClientUnity.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public class ContractNear : DynamicObject, IDynamicMetaObjectProvider
    {
        private readonly Account _account;
        private readonly string _contractId;
        private readonly string[] _availableChangeMethods;
        private readonly string[] _availableViewMethods;

        public ContractNear(Account account, string contractId, ContractOptions options)
        {
            _account = account;
            _contractId = contractId;
            _availableViewMethods = options.viewMethods as string[];
            _availableChangeMethods = options.changeMethods as string[];
        }

        public async Task<dynamic> Change(string methodName, dynamic args, ulong? gas = null, Nullable<UInt128> amount = null)
        {
            var rawResult = await _account.FunctionCallAsync(_contractId, methodName, args, gas, amount);
            return Provider.GetTransactionLastResult(rawResult);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out dynamic result)
        {
            if (Array.Exists(_availableChangeMethods, changeMethod => changeMethod == binder.Name))
            {
                if (args.Length == 0)
                {
                    dynamic changeMethodnArgs = new ExpandoObject();
                    result = Change(binder.Name, changeMethodnArgs);
                    return true;
                }
                if (args.Length == 1 && args[0].GetType() == typeof(ExpandoObject))
                {
                    result = Change(binder.Name, args[0]);
                    return true;
                }
                else if (args.Length == 2 && args[0].GetType() == typeof(ExpandoObject) && args[1].GetType() == typeof(ulong))
                {
                    result = Change(binder.Name, args[0], (ulong)args[1]);
                    return true;
                }
                else if (args.Length == 2 && args[0].GetType() == typeof(ExpandoObject) && args[1].GetType() == typeof(UInt128))
                {
                    result = Change(binder.Name, args[0], (Nullable<ulong>)null, (UInt128)args[1]);
                    return true;
                }
                else if (args.Length == 3 && args[0].GetType() == typeof(ExpandoObject) && args[1].GetType() == typeof(ulong) && args[2].GetType() == typeof(UInt128))
                {
                    result = Change(binder.Name, args[0], (ulong)args[1], (UInt128)args[2]);
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            else if (Array.Exists(_availableViewMethods, viewMethod => viewMethod == binder.Name))
            {
                if (args.Length == 0)
                {
                    dynamic viewMethodnArgs = new ExpandoObject();
                    result = View(binder.Name, viewMethodnArgs);
                    return true;
                }
                if (args.Length == 1 && args[0].GetType() == typeof(ExpandoObject))
                {
                    result = View(binder.Name, args[0]);
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                result = null;
                return false;
            }
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