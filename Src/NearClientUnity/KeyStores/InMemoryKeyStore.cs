using NearClientUnity.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NearClientUnity.KeyStores
{
    public class InMemoryKeyStore : KeyStore
    {
        private readonly Dictionary<string, string> _keys;

        public InMemoryKeyStore()
        {
            _keys = new Dictionary<string, string>();
        }

        public override async Task ClearAsync()
        {
            await Task.Factory.StartNew(() => { _keys.Clear(); });
        }

        public override async Task<string[]> GetAccountsAsync(string networkId)
        {
            return await Task.Factory.StartNew(() =>
            {
                var allAccounts = new List<string>();
                var keys = _keys.Keys.ToArray();
                if (keys.Length == 0) return new string[0];
                foreach (var key in keys)
                {
                    var parts = key.Split(':');
                    if (parts[parts.Length - 1] != networkId) continue;
                    Array.Resize(ref parts, parts.Length - 1);
                    allAccounts.Add(string.Join(":", parts));
                }
                var accounts = new HashSet<string>(allAccounts).ToArray();
                return accounts;
            });
        }

        public override async Task<KeyPair> GetKeyAsync(string networkId, string accountId)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    var value = _keys[$"{accountId}:{networkId}"];
                    var result = KeyPair.FromString(value);
                    return result;
                }
                catch (KeyNotFoundException)
                {
                    return null;
                }
            });
        }

        public override async Task<string[]> GetNetworksAsync()
        {
            return await Task.Factory.StartNew(() =>
            {
                var keys = _keys.Keys.ToArray();
                if (keys.Length == 0) return new string[0];
                var allNetworks = keys.Select(key => key.Split(':')[1]).ToList();
                var networks = new HashSet<string>(allNetworks).ToArray();
                return networks;
            });
        }

        public override async Task RemoveKeyAsync(string networkId, string accountId)
        {
            await Task.Factory.StartNew(() => { _keys.Remove($"{accountId}:{networkId}"); });
        }

        public override async Task SetKeyAsync(string networkId, string accountId, KeyPair keyPair)
        {
            await Task.Factory.StartNew(() =>
            {
                _keys[$"{accountId}:{networkId}"] = keyPair.ToString();
            });
        }
    }
}