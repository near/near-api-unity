using System;

namespace NearClientUnity.Utilities
{
    public class PublicKey
    {
        private KeyType _keyType;
        private readonly ByteArray32 _data;

        internal ByteArray32 Data => _data;

        public PublicKey(KeyType keyType, ByteArray32 data)
        {
            _keyType = keyType;
            _data = data;
        }

        public PublicKey(string encodedKey)
        {
            var parts = encodedKey.Split(':');
            switch (parts.Length)
            {
                case 1:
                {
                    var decodeData = Base58.Decode(parts[0]);
                    if (decodeData.Length != ByteArray32.BufferLength)
                        throw new ArgumentException("Invalid encoded key");
                    var byteArray32 = new ByteArray32 {Buffer = decodeData};
                    _keyType = KeyType.Ed25519;
                    _data = byteArray32;
                    break;
                }
                case 2:
                {
                    var decodeData = Base58.Decode(parts[1]);
                    if (decodeData.Length != 32) throw new ArgumentException("Invalid encoded key");
                    var byteArray32 = new ByteArray32 {Buffer = decodeData};
                    _keyType = KeyTypeConverter.StringToKeyType(parts[0]);
                    _data = byteArray32;
                    break;
                }
                default:
                    throw new NotSupportedException("Invalid encoded key format, must be '<curve>:<encoded key>'");
            }
        }
    }
}