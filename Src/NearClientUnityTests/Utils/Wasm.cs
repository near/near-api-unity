using System.IO;
using System.Reflection;

namespace NearClientUnityTests.Utils
{
    public static class Wasm
    {
        public static byte[] GetBytes()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream("NearClientUnityTests.Utils.main.wasm"))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }
    }
}