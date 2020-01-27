using NearClientUnity.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace NearClientUnity.Providers
{
    public abstract class Provider
    {
        public static dynamic GetTransactionLastResult(FinalExecutionOutcome txResult)
        {
            if (txResult.Status == null || txResult.Status.GetType() != typeof(FinalExecutionStatus) || string.Equals(
                    txResult.Status.SuccessValue, null, StringComparison.Ordinal)) return null;
            Console.WriteLine("GetTransactionLastResult " + txResult.Status.SuccessValue);
            var valueBytes = Convert.FromBase64String(txResult.Status.SuccessValue);
            string valueString = System.Text.Encoding.UTF8.GetString(valueBytes).Trim('"');
            try
            {
                var result = JObject.Parse(valueString);
                Console.WriteLine("GetTransactionLastResult value " + valueString + "/ result " + result);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetTransactionLastResult exception " + ex + " value " + valueString);
                return valueString;
            }
        }

        public abstract Task<BlockResult> GetBlockAsync(int blockId);

        public abstract Task<ChunkResult> GetChunkAsync(string chunkId);

        public abstract Task<ChunkResult> GetChunkAsync(int[,] chunkId);

        public abstract INetwork GetNetwork();

        public abstract Task<NodeStatusResult> GetStatusAsync();

        public abstract Task<FinalExecutionOutcome> GetTxStatusAsync(byte[] txHash, string accountId);

        public abstract Task<dynamic> QueryAsync(string path, string data);

        public abstract Task<FinalExecutionOutcome> SendTransactionAsync(SignedTransaction signedTransaction);
    }
}