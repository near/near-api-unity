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

        public static AccessKey FromDynamicJsonObject(dynamic jsonObject)
        {
            
            if (jsonObject.permission.GetType().Name == "JValue" && jsonObject.permission.Value.GetType().Name == "String" && jsonObject.permission.Value == "FullAccess")
            {
                return new AccessKey
                {
                    Nonce = jsonObject.nonce,
                    Permission = new AccessKeyPermission
                    {
                        PermissionType = AccessKeyPermissionType.FullAccessPermission,
                        FullAccess = new FullAccessPermission()
                    }
                };
            }
            else
            {
                var rawAllowance = jsonObject.permission.FunctionCall.allowance ?? null;
                return new AccessKey
                {
                    Nonce = jsonObject.nonce,
                    Permission = new AccessKeyPermission
                    {
                        PermissionType = AccessKeyPermissionType.FunctionCallPermission,
                        FunctionCall = new FunctionCallPermission()
                        {
                            Allowance = rawAllowance == null ? UInt128.Parse(rawAllowance) : null,
                            MethodNames = jsonObject.permission.FunctionCall.method_names.ToObject<string[]>(),
                            ReceiverId = jsonObject.permission.FunctionCall.receiver_id,
                        }
                    }
                };
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