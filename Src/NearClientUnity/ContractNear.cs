using NearClientUnity.Providers;
using NearClientUnity.Utilities;
using System;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public class ContractNear
    {
        private readonly Account _account;
        private readonly dynamic _changeMethods;
        private readonly string _contractId;
        private readonly dynamic _viewMethods;

        public ContractNear(Account account, string contractId, ContractOptions options)
        {
            _account = account;
            _contractId = contractId;
            foreach (var vm in options.viewMethods)
            {
                _viewMethods[vm] = new Func<dynamic, Task<dynamic>>(async (dynamic args) =>
                {
                    var result = await _account.ViewFunctionAsync(_contractId, vm, args);
                    return result;
                });
            }

            foreach (var cm in options.changeMethods)
            {
                _changeMethods[cm] = new Func<dynamic, ulong, UInt128, Task<dynamic>>(
                    async (dynamic args, ulong gas, UInt128 amount) =>
                    {
                        var rawResult = await _account.FunctionCallAsync(_contractId, cm, args, gas, amount);
                        var result = Provider.GetTransactionLastResult(rawResult);
                        return result;
                    });
            }
        }
    }
}