using System.Security.Cryptography;

namespace NearClientUnity.Utilities
{
    public class KeyPairEd25519 : KeyPair
    {
        private readonly byte[] _expandedSecretKey;
        private readonly PublicKey _publicKey;
        private readonly string _secretKey;

        public KeyPairEd25519(string secretKey)
        {
            var publicKeyFromSeed = Ed25519.Ed25519.PublicKeyFromSeed(Base58.Decode(secretKey));
            var publicKeyFromSeed32 = new ByteArray32 { Buffer = publicKeyFromSeed };

            _publicKey = new PublicKey(KeyType.Ed25519, publicKeyFromSeed32);
            _secretKey = secretKey;
            _expandedSecretKey = Ed25519.Ed25519.ExpandedPrivateKeyFromSeed(Base58.Decode(secretKey));
        }

        public static KeyPairEd25519 FromRandom()
        {
            var randomSecretKey = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomSecretKey);
            }

            return new KeyPairEd25519(Base58.Encode(randomSecretKey));
        }

        public override PublicKey GetPublicKey()
        {
            return _publicKey;
        }

        public string GetSecretKey()
        {
            return _secretKey;
        }

        public override Signature Sign(byte[] message)
        {
            var signature = Ed25519.Ed25519.Sign(message, _expandedSecretKey);
            var sign = new Signature { SignatureBytes = signature, PublicKey = this._publicKey };
            return sign;
        }

        public override string ToString()
        {
            return $"ed25519:{_secretKey}";
        }

        public override bool Verify(byte[] message, byte[] signature)
        {
            return Ed25519.Ed25519.Verify(signature, message, _publicKey.Data.Buffer);
        }
    }
}