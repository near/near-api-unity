using NearClientUnity.Utilities;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public abstract class AccountCreator
    {
        public abstract Task CreateAccountAsync(string newAccountId, PublicKey publicKey);
    }
}