using NearClientUnity;
using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using NUnit.Framework;

namespace NearClientUnityTests
{
    [TestFixture]
    public class SignerTests
    {
        [Test]
        public void ShouldThrowNoKey()
        {
            var signer = new InMemorySigner(new InMemoryKeyStore());
            var decodedMessage = Base58.Decode("message");
            Assert.That(() => signer.SignMessageAsync(decodedMessage, "user", "network"), Throws.Exception.TypeOf<InMemorySignerException>());
        }
    }
}