using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class Signature
    {
        private KeyType _keyType;
        private byte[] _data;

        public KeyType KeyType => _keyType;
        public byte[] Data => _data;

        public Signature(byte[] signature)
        {
            _keyType = KeyType.Ed25519;
            _data = signature;
        }
    }
}