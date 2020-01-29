using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using System;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public class InMemorySigner : Signer
    {
        private readonly KeyStore _keyStore;

        public InMemorySigner(KeyStore keyStore)
        {
            _keyStore = keyStore;
        }

        public KeyStore KeyStore => _keyStore;

        public override async Task<PublicKey> CreateKeyAsync(string accountId, string networkId = "")
        {
            var keyPair = KeyPair.FromRandom("ed25519");
            await _keyStore.SetKeyAsync(networkId, accountId, keyPair);
            return keyPair.GetPublicKey();
        }

        public override async Task<PublicKey> GetPublicKeyAsync(string accountId = "", string networkId = "")
        {
            var keyPair = await _keyStore.GetKeyAsync(networkId, accountId);
            return keyPair?.GetPublicKey();
        }

        public override async Task<Utilities.Signature> SignHashAsync(byte[] hash, string accountId = "", string networkId = "")
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrWhiteSpace(accountId))
            {
                throw new ArgumentException("InMemorySigner requires provided account id");
            }
            var keyPair = await _keyStore.GetKeyAsync(networkId, accountId);
            if (keyPair == null)
            {
                throw new InMemorySignerException($"Key for { accountId} not found in { networkId}");
            }
            return keyPair.Sign(hash);
        }
    }

    public class InMemorySignerException : Exception
    {
        public InMemorySignerException()
        {
        }

        public InMemorySignerException(string name)
            : base(name)
        {
        }
    }
}