using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine.Networking.PlayerConnection;

namespace NearClientUnity
{
    public class SignedTransaction
    {
        public Transaction Transaction { get; set; }
        public NearSignature Signature { get; set; }

        public virtual byte[] Encode()
        {
            var result = new byte[3];
            return result;
        }

        public static async Task<Tuple<byte[], SignedTransaction>> SignTransaction(string receiverId, int nonce, Action[] actions, byte[] blockHash, Signer signer, string accountId, string networkId)
        {
            var publicKey = await signer.GetPublicKeyAsync(accountId, networkId);
            var transaction = new Transaction
            {
                SignerId = accountId,
                PublicKey = publicKey,
                Nonce = nonce,
                ReceiverId = receiverId,
                Actions = actions,
                BlockHash = blockHash
            };
            var message = new byte[32]; //ToDo: Need implementation serialize
            
            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(message);
            }

            var signature = await signer.SignMessageAsync(message, accountId, networkId);

            var signedTx = new SignedTransaction
            {
                Transaction = transaction,
                Signature = new NearSignature(signature.SignatureBytes)
            };

            var result = new Tuple<byte[], SignedTransaction>(hash, signedTx);
            return result;
        }

        public static async Task<Tuple<byte[], SignedTransaction>> SignTransaction(string receiverId, int nonce, Action[] actions, byte[] blockHash, Signer signer, string accountId)
        {
            var publicKey = await signer.GetPublicKeyAsync(accountId);
            var transaction = new Transaction
            {
                SignerId = accountId,
                PublicKey = publicKey,
                Nonce = nonce,
                ReceiverId = receiverId,
                Actions = actions,
                BlockHash = blockHash
            };
            var message = new byte[32]; //ToDo: Need implementation serialize

            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(message);
            }

            var signature = await signer.SignMessageAsync(message, accountId);

            var signedTx = new SignedTransaction
            {
                Transaction = transaction,
                Signature = new NearSignature(signature.SignatureBytes)
            };

            var result = new Tuple<byte[], SignedTransaction>(hash, signedTx);
            return result;
        }

        public static async Task<Tuple<byte[], SignedTransaction>> SignTransaction(string receiverId, int nonce, Action[] actions, byte[] blockHash, Signer signer)
        {
            var publicKey = await signer.GetPublicKeyAsync();
            var transaction = new Transaction
            {
                PublicKey = publicKey,
                Nonce = nonce,
                ReceiverId = receiverId,
                Actions = actions,
                BlockHash = blockHash
            };
            var message = new byte[32]; //ToDo: Need implementation serialize

            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(message);
            }

            var signature = await signer.SignMessageAsync(message);

            var signedTx = new SignedTransaction
            {
                Transaction = transaction,
                Signature = new NearSignature(signature.SignatureBytes)
            };

            var result = new Tuple<byte[], SignedTransaction>(hash, signedTx);
            return result;
        }
    }
}