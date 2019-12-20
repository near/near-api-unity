using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;

namespace NearClientUnity
{
    public class NearConfig : ConnectionConfig
    {
        public string NodeUrl { get; set; }
        public string MasterAccount { get; set; }
        public string KeyPath { get; set; } 
        public string HelperUrl { get; set; }
        public UInt128 InitialBalance { get; set; }
        public ProviderType ProviderType { get; set; }
        public SignerType SignerType { get; set; }
        public KeyStore KeyStore { get; set; }
        public string  ContractName { get; set; }
        public string  WalletUrl { get; set; }
    }
}