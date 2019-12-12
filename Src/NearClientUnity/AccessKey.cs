using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class AccessKey
    {
        public int Nonce { get; set; }
        public AccessKeyPermission Permission { get; set; }

        public static AccessKey FullAccessKey()
        {
            var key = new AccessKey
            {
                Nonce = 0,
                Permission = new AccessKeyPermission
                {
                    FullAccess = new FullAccessPermission()
                }
            };
            return key;
        }

        public static AccessKey FunctionCallAccessKey(string receiverId, string[] methodNames, UInt128 allowance) 
        {
            var key = new AccessKey
            { 
                Nonce = 0,
                Permission = new AccessKeyPermission
                { 
                    FunctionCall = new FunctionCallPermission
                    {
                        ReceiverId = receiverId, Allowance = allowance, MethodNames = methodNames
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
                    FunctionCall = new FunctionCallPermission
                    {
                        ReceiverId = receiverId,
                        MethodNames = methodNames
                    }

                }

            };
            return key;
        }
    }
}