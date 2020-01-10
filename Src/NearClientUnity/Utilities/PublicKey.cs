using System;
using System.IO;

namespace NearClientUnity.Utilities
{
    public class PublicKey
    {
        private readonly ByteArray32 _data;
        private KeyType _keyType;

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
                        var byteArray32 = new ByteArray32 { Buffer = decodeData };
                        _keyType = KeyType.Ed25519;
                        _data = byteArray32;
                        break;
                    }
                case 2:
                    {
                        var decodeData = Base58.Decode(parts[1]);
                        if (decodeData.Length != 32) throw new ArgumentException("Invalid encoded key");
                        var byteArray32 = new ByteArray32 { Buffer = decodeData };
                        _keyType = KeyTypeConverter.StringToKeyType(parts[0]);
                        _data = byteArray32;
                        break;
                    }
                default:
                    throw new NotSupportedException("Invalid encoded key format, must be '<curve>:<encoded key>'");
            }
        }

        internal ByteArray32 Data => _data;

        public static PublicKey FromByteArray(byte[] rawBytes)
        {
            if (rawBytes.Length != 33) throw new ArgumentException("Invalid raw bytes for public key");
            using (var ms = new MemoryStream(rawBytes))
            {
                return FromStream(ms);
            }
        }

        public static PublicKey FromStream(MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static PublicKey FromStream(ref MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    writer.Write((byte)_keyType);
                    writer.Write(_data.Buffer);
                    return ms.ToArray();
                }
            }
        }

        public override string ToString()
        {
            var key = Base58.Encode(_data.Buffer);
            var type = KeyTypeConverter.KeyTypeToString(_keyType);
            return $"{type}:{key}";
        }

        private static PublicKey FromRawDataStream(MemoryStream stream)
        {
            using (var reader = new NearBinaryReader(stream, true))
            {
                KeyType keyType;
                switch ((int)reader.ReadByte())
                {
                    case 0:
                        {
                            keyType = KeyType.Ed25519;
                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException("Invalid key type in raw bytes for public key");
                        }
                }

                var data = new ByteArray32
                {
                    Buffer = reader.ReadBytes(32)
                };

                return new PublicKey(keyType, data);
            }
        }
    }
}