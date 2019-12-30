using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class NearSignature
    {
        private byte[] _data;
        private KeyType _keyType;

        public NearSignature(byte[] signature)
        {
            _keyType = KeyType.Ed25519;
            _data = signature;
        }

        public byte[] Data => _data;
        public KeyType KeyType => _keyType;
    }
}