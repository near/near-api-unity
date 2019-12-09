using System;

namespace NearClientUnity
{
    public abstract class SignedTransaction
    {
        public abstract Transaction Transaction { get; set; }
        public abstract Signature Signature { get; set; }

        public virtual byte[] Encode()
        {
            var result = new byte[3];
            return result;
        }
    }
}