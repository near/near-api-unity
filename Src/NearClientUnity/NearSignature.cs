using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class NearSignature
    {
        private KeyType _keyType;
        private byte[] _data;

        public KeyType KeyType => _keyType;
        public byte[] Data => _data;

        public NearSignature(byte[] signature)
        {
            _keyType = KeyType.Ed25519;
            _data = signature;
        }
    }
}