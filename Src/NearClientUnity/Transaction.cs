using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public abstract class Transaction
    {
        public abstract string SignerId { get; set; }
        public abstract PublicKey PublicKey { get; set; }
        public abstract int Nonce { get; set; }
        public abstract string ReceiverId { get; set; }
        public abstract Action[] Actions { get; set; }
        public abstract byte[] BlockHash { get; set; }
    }
}