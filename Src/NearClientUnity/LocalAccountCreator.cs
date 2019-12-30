using NearClientUnity.Utilities;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public class LocalAccountCreator : AccountCreator
    {
        private readonly UInt128 _initialBalance;
        private readonly Account _masterAccount;

        public LocalAccountCreator(Account masterAccount, UInt128 initialBalance)
        {
            _masterAccount = masterAccount;
            _initialBalance = initialBalance;
        }

        public override async Task CreateAccountAsync(string newAccountId, PublicKey publicKey)
        {
            await _masterAccount.CreateAccountAsync(newAccountId, publicKey, _initialBalance);
        }
    }
}