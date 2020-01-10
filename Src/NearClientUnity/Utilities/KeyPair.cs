using System;

namespace NearClientUnity.Utilities
{
    public abstract class KeyPair
    {
        public static KeyPair FromRandom(string curve)
        {
            switch (curve.ToUpper())
            {
                case "ED25519":
                    return KeyPairEd25519.FromRandom();

                default:
                    throw new NotSupportedException($"Unknown curve {curve}");
            }
        }

        public static KeyPair FromString(string encodedKey)
        {
            var parts = encodedKey.Split(':');
            switch (parts.Length)
            {
                case 1:
                    {
                        return new KeyPairEd25519(parts[0]);
                    }
                case 2:
                    {
                        switch (parts[0].ToUpper())
                        {
                            case "ED25519":
                                return new KeyPairEd25519(parts[1]);

                            default:
                                throw new NotSupportedException($"Unknown curve {parts[0]}");
                        }
                    }
                default:
                    throw new NotSupportedException("Invalid encoded key format, must be '<curve>:<encoded key>'");
            }
        }

        public abstract PublicKey GetPublicKey();

        public abstract Signature Sign(byte[] message);

        public abstract bool Verify(byte[] message, byte[] signature);
    }
}