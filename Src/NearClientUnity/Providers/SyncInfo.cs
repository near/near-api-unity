namespace NearClientUnity.Providers
{
    public abstract class SyncInfo
    {
        public abstract string LatestBlockHash { get; set; }
        public abstract int LatestBlockHeight { get; set; }
        public abstract string LatestBlockTime { get; set; }
        public abstract string LatestStateRoot { get; set; }
        public abstract bool Syncing { get; set; }
    }
}