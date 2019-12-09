using System.Threading.Tasks;
using NearClientUnity.Utilities;

namespace NearClientUnity.Providers
{
    public abstract class Provider
    {
        public abstract INetwork GetNetwork();
        public abstract Task<NodeStatusResult> GetStatusAsync();

        public abstract Task<FinalExecutionOutcome> SendTransactionAsync(SignedTransaction signedTransaction);
        public abstract Task<FinalExecutionOutcome> GetTxStatusAsync(byte[] txHash, string accountId);
        public abstract Task<dynamic> QueryAsync(string path, string data);
        public abstract Task<BlockResult> GetBlockAsync(int blockId);
        public abstract Task<ChunkResult> GetChunkAsync(string chunkId);
        public abstract Task<ChunkResult> GetChunkAsync(int[,] chunkId);
    }
}