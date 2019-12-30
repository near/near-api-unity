using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class FunctionCallPermission
    {
        public UInt128 Allowance { get; set; }
        public string[] MethodNames { get; set; }
        public string ReceiverId { get; set; }
    }
}