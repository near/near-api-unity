using NUnit.Framework;
using System;
using NearClientUnity.Utilities;

namespace NearClientUnityTests.Utils
{
    [TestFixture]
    public class KeyPairTests
    {
        [Test]
        public void ItShouldSignAndVerify()
        {
            var keypair = new KeyPairEd25519(secretKey:"26x56YPzPDro5t2smQfGcYAPy3j7R2jB2NUb7xKbAGK23B6x4WNQPh3twb6oDksFov5X8ts5CtntUNbpQpAKFdbR");
            Console.WriteLine("GetPublicKey - " + keypair.GetPublicKey());
            Assert.AreEqual(keypair.GetPublicKey().ToString(), "ed25519:AYWv9RAN1hpSQA4p1DLhCNnpnNXwxhfH9qeHN8B4nJ59");
        }
    }    
}
