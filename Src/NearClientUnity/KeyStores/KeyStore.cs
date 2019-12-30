using NearClientUnity.Utilities;
using System.Threading.Tasks;

namespace NearClientUnity.KeyStores
{
    public abstract class KeyStore
    {
        public abstract Task ClearAsync();

        public abstract Task<string[]> GetAccountsAsync(string networkId);

        public abstract Task<KeyPair> GetKeyAsync(string networkId, string accountId);

        public abstract Task<string[]> GetNetworksAsync();

        public abstract Task RemoveKeyAsync(string networkId, string accountId);

        public abstract Task SetKeyAsync(string networkId, string accountId, KeyPair keyPair);
    }
}