using NearClientUnity.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NearClientUnity.KeyStores
{
    public class MergeKeyStore : KeyStore
    {
        private KeyStore[] _keyStores;

        public MergeKeyStore(KeyStore[] keyStores)
        {
            _keyStores = keyStores;
        }

        public KeyStore[] KeyStores => _keyStores;
        public KeyStore[] Stores => _keyStores;

        public override async Task ClearAsync()
        {
            foreach (var keyStore in _keyStores)
            {
                await keyStore.ClearAsync();
            }
        }

        public override async Task<string[]> GetAccountsAsync(string networkId)
        {
            var result = new HashSet<string>();
            foreach (var keyStore in _keyStores)
            {
                foreach (var account in await keyStore.GetAccountsAsync(networkId))
                {
                    result.Add(account);
                }
            }

            return result.ToArray();
        }

        public override async Task<KeyPair> GetKeyAsync(string networkId, string accountId)
        {
            foreach (var keyStore in _keyStores)
            {
                var keyPair = await keyStore.GetKeyAsync(networkId, accountId);
                if (keyPair != null) return keyPair;
            }

            return null;
        }

        public override async Task<string[]> GetNetworksAsync()
        {
            var result = new HashSet<string>();
            foreach (var keyStore in _keyStores)
            {
                foreach (var network in await keyStore.GetNetworksAsync())
                {
                    result.Add(network);
                }
            }

            return result.ToArray();
        }

        public override async Task RemoveKeyAsync(string networkId, string accountId)
        {
            foreach (var keyStore in _keyStores)
            {
                await keyStore.RemoveKeyAsync(networkId, accountId);
            }
        }

        public override async Task SetKeyAsync(string networkId, string accountId, KeyPair keyPair)
        {
            await _keyStores[0].SetKeyAsync(networkId, accountId, keyPair);
        }
    }
}