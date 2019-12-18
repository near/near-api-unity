using System;
using System.Threading.Tasks;
using NearClientUnity.Utilities;
using Newtonsoft.Json.Linq;

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

        public static dynamic GetTransactionLastResult(FinalExecutionOutcome txResult)
        {
            if (txResult.Status == null || txResult.Status.GetType() != typeof(object) || string.Equals(
                    txResult.Status.SuccessValue, null, StringComparison.Ordinal)) return null;
            var value = Convert.FromBase64String(txResult.Status.SuccessValue).ToString();

            try
            {
                var result = JObject.Parse(value);
                return result;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}