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
            var keypair = new KeyPairEd25519(secretKey: "26x56YPzPDro5t2smQfGcYAPy3j7R2jB2NUb7xKbAGK23B6x4WNQPh3twb6oDksFov5X8ts5CtntUNbpQpAKFdbR");
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

        [Test]
        public void ItShouldSignAndVerifyWithRandom()
        {
            var keypair = KeyPairEd25519.FromRandom();
            byte[] messageBytes = Encoding.Default.GetBytes("message");
            byte[] messageSha256;
            using (var sha256 = SHA256.Create())
            {
                messageSha256 = sha256.ComputeHash(messageBytes);
            }
            var signature = keypair.Sign(messageSha256);
            Assert.AreEqual(keypair.Verify(messageSha256, signature.SignatureBytes), true);
        }

        [Test]
        public void ItShouldInitFromSecret()
        {
            var keypair = new KeyPairEd25519(secretKey: "5JueXZhEEVqGVT5powZ5twyPP8wrap2K7RdAYGGdjBwiBdd7Hh6aQxMP1u3Ma9Yanq1nEv32EW7u8kUJsZ6f315C");
            Assert.AreEqual(keypair.GetPublicKey().ToString(), "ed25519:EWrekY1deMND7N3Q7Dixxj12wD7AVjFRt2H9q21QHUSW");
        }

        [Test]
        public void ItShouldConvertToString()
        {
            var keypair = KeyPairEd25519.FromRandom();
            var keypairFromString = (KeyPairEd25519)KeyPair.FromString(keypair.ToString());
            Assert.AreEqual(keypairFromString.GetSecretKey(), keypair.GetSecretKey());
            var key = "ed25519:2wyRcSwSuHtRVmkMCGjPwnzZmQLeXLzLLyED1NDMt4BjnKgQL6tF85yBx6Jr26D2dUNeC716RBoTxntVHsegogYw";
            var keypairFromKeyString = KeyPair.FromString(key);
            Assert.AreEqual(keypairFromKeyString.ToString(), key);
        }
    }
}
