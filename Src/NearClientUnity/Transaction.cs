using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class Transaction
    {
        public string SignerId { get; set; }
        public PublicKey PublicKey { get; set; }
        public int Nonce { get; set; }
        public string ReceiverId { get; set; }
        public Action[] Actions { get; set; }
        public byte[] BlockHash { get; set; }
    }
}