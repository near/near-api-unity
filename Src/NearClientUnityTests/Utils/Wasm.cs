using System;
using System.IO;
using NUnit.Framework;

namespace NearClientUnityTests.Utils
{
    public class Wasm
    {
        public static byte[] GetBytes()
        {
            return File.ReadAllBytes(TestContext.CurrentContext.TestDirectory + "\\Utils\\main.wasm");
        }
    }
}
