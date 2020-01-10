using NearClientUnity.Utilities;
using System.IO;

namespace NearClientUnity
{
    public class AccessKey
    {
        public ulong Nonce { get; set; }
        public AccessKeyPermission Permission { get; set; }

        public static AccessKey FromByteArray(byte[] rawBytes)
        {
            using (var ms = new MemoryStream(rawBytes))
            {
                return FromStream(ms);
            }
        }

        public static AccessKey FromStream(MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static AccessKey FromStream(ref MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static AccessKey FullAccessKey()
        {
            var key = new AccessKey
            {
                Nonce = 0,
                Permission = new AccessKeyPermission
                {
                    PermissionType = AccessKeyPermissionType.FullAccessPermission,
                    FullAccess = new FullAccessPermission()
                }
            };
            return key;
        }

        public static AccessKey FunctionCallAccessKey(string receiverId, string[] methodNames, UInt128? allowance)
        {
            var key = new AccessKey
            {
                Nonce = 0,
                Permission = new AccessKeyPermission
                {
                    PermissionType = AccessKeyPermissionType.FunctionCallPermission,
                    FunctionCall = new FunctionCallPermission
                    {
                        ReceiverId = receiverId,
                        Allowance = allowance,
                        MethodNames = methodNames
                    }
                }
            };
            return key;
        }

        public static AccessKey FunctionCallAccessKey(string receiverId, string[] methodNames)
        {
            var key = new AccessKey
            {
                Nonce = 0,
                Permission = new AccessKeyPermission
                {
                    PermissionType = AccessKeyPermissionType.FunctionCallPermission,
                    FunctionCall = new FunctionCallPermission
                    {
                        ReceiverId = receiverId,
                        MethodNames = methodNames
                    }
                }
            };
            return key;
        }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    writer.Write(Nonce);
                    writer.Write(Permission.ToByteArray());
                    return ms.ToArray();
                }
            }
        }

        private static AccessKey FromRawDataStream(MemoryStream stream)
        {
            using (var reader = new NearBinaryReader(stream, true))
            {
                var nonce = reader.ReadULong();
                var permission = AccessKeyPermission.FromStream(ref stream);

                return new AccessKey()
                {
                    Nonce = nonce,
                    Permission = permission
                };
            }
        }
    }
}