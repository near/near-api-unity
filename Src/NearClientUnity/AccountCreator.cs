using System.Threading.Tasks;
using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public abstract class AccountCreator
    {
        public abstract Task createAccount(string newAccountId, PublicKey publicKey);
    }
}