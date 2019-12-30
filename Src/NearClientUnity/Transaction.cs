using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class Transaction
    {
        public Action[] Actions { get; set; }
        public byte[] BlockHash { get; set; }
        public int Nonce { get; set; }
        public PublicKey PublicKey { get; set; }
        public string ReceiverId { get; set; }
        public string SignerId { get; set; }
    }
}