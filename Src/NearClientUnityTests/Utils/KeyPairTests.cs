using NUnit.Framework;
using NearClientUnity.Utilities;
using System.Text;
using System.Security.Cryptography;

namespace NearClientUnityTests.Utils
{
    [TestFixture]
    public class KeyPairTests
    {
        [Test]
        public void ItShouldSignAndVerify()
        {
            var keypair = new KeyPairEd25519(secretKey:"26x56YPzPDro5t2smQfGcYAPy3j7R2jB2NUb7xKbAGK23B6x4WNQPh3twb6oDksFov5X8ts5CtntUNbpQpAKFdbR");            
            Assert.AreEqual(keypair.GetPublicKey().ToString(), "ed25519:AYWv9RAN1hpSQA4p1DLhCNnpnNXwxhfH9qeHN8B4nJ59");
            byte[] messageBytes = Encoding.Default.GetBytes("message");
            byte[] messageSha256;
            using (var sha256 = SHA256.Create())
            {
                messageSha256 = sha256.ComputeHash(messageBytes);
            }
            var signature = keypair.Sign(messageSha256);
            var signedMessage = Base58.Encode(signature.SignatureBytes);
            Assert.AreEqual(signedMessage, "26gFr4xth7W9K7HPWAxq3BLsua8oTy378mC1MYFiEXHBBpeBjP8WmJEJo8XTBowetvqbRshcQEtBUdwQcAqDyP8T");
        }
    }    
}
