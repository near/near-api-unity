using NearClientUnity.Utilities;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public abstract class Signer
    {
        /**
        * Creates new key and returns public key.
        */

        public abstract Task<PublicKey> CreateKeyAsync(string accountId, string networkId = "");

        /**
         * Returns public key for given account / network.
         * @param accountId accountId to retrieve from.
         * @param networkId network for this accountId.
         */

        public abstract Task<PublicKey> GetPublicKeyAsync(string accountId = "", string networkId = "");

        /**
         * Signs given hash.
         * @param hash hash to sign.
         * @param accountId accountId to use for signing.
         * @param networkId network for this accontId.
         */

        public abstract Task<Utilities.Signature> SignHashAsync(byte[] hash, string accountId = "", string networkId = "");

        /**
         * Signs given message, by first hashing with sha256.
         * @param message message to sign.
         * @param accountId accountId to use for signing.
         * @param networkId network for this accontId.
         */

        public async Task<Utilities.Signature> SignMessageAsync(byte[] message, string accountId = "", string networkId = "")
        {
            byte[] messageSha256;

            using (var sha256 = SHA256.Create())
            {
                messageSha256 = sha256.ComputeHash(message);
            }

            var result = await SignHashAsync(messageSha256, accountId, networkId);
            return result;
        }
    }
}