using NearClientUnity.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace NearClientUnity.Providers
{
    public abstract class Provider
    {
        public static dynamic GetTransactionLastResult(FinalExecutionOutcome txResult)
        {
            //Console.WriteLine("txResult >>>>>>>>>>>>> :" + txResult);
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(txResult))
            {
                string name = descriptor.Name;
                object val = descriptor.GetValue(txResult);
                //Console.WriteLine("{0}={1}", name, val);
            }
            //Console.WriteLine("");

            if (txResult.Status == null || txResult.Status.GetType() != typeof(FinalExecutionStatus) || string.Equals(
                    txResult.Status.SuccessValue, null, StringComparison.Ordinal)) return null;

            var value = Encoding.UTF8.GetString(Convert.FromBase64String(txResult.Status.SuccessValue)).Trim('"');

            try
            {
                var result = JObject.Parse(value);
                //Console.WriteLine("GetTransactionLastResult value " + value + "/ result " + result);
                return result;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("GetTransactionLastResult exception " + ex + " value " + value);
                return value;
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