using System.Threading.Tasks;
using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class LocalAccountCreator : AccountCreator
    {
        private readonly Account _masterAccount;
        private readonly UInt128 _initialBalance;

        public LocalAccountCreator(Account masterAccount, UInt128 initialBalance)
        {
            _masterAccount = masterAccount;
            _initialBalance = initialBalance;
        }

        public override async Task createAccount(string newAccountId, PublicKey publicKey)
        {
            await _masterAccount.CreateAccountAsync(newAccountId, publicKey, _initialBalance);
        }
    }
}