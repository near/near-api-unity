namespace NearClientUnity.Providers
{
    public abstract class NodeStatusResult
    {
        public abstract string ChainId { get; set; }
        public abstract string RpcAddr { get; set; }
        public abstract SyncInfo SyncInfo { get; set; }
        public abstract string[] Validators { get; set; }
    }
}