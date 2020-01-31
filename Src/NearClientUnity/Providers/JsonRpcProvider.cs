using NearClientUnity.Utilities;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Threading.Tasks;
using System.Web;

namespace NearClientUnity.Providers
{
    public class JsonRpcProvider : Provider
    {
        private readonly ConnectionInfo _connection;

        public JsonRpcProvider(string url)
        {
            var connectionInfo = new ConnectionInfo
            {
                Url = url
            };
            _connection = connectionInfo;
        }

        private int _id { get; set; } = 123;

        public override async Task<BlockResult> GetBlockAsync(int blockId)
        {
            var parameters = new dynamic[1];
            parameters[0] = blockId;
            var result = await SendJsonRpc("block", parameters);
            return result;
        }

        public override async Task<ChunkResult> GetChunkAsync(string chunkId)
        {
            var parameters = new dynamic[1];
            parameters[0] = chunkId;
            var result = await SendJsonRpc("chunk", parameters);
            return result;
        }

        public override Task<ChunkResult> GetChunkAsync(int[,] chunkId)
        {
            throw new NotImplementedException();
        }

        public override INetwork GetNetwork()
        {
            INetwork result = null;

            result.Name = "test";
            result.ChainId = "test";

            return result;
        }

        public override async Task<NodeStatusResult> GetStatusAsync()
        {
            var rawStatusResul = await SendJsonRpc("status", new dynamic[0]);
            var result = NodeStatusResult.FromDynamicJsonObject(rawStatusResul);
            return result;
        }

        public override async Task<FinalExecutionOutcome> GetTxStatusAsync(byte[] txHash, string accountId)
        {
            var parameters = new dynamic[2];
            parameters[0] = Base58.Encode(txHash);
            parameters[1] = accountId;
            var result = await SendJsonRpc("tx", parameters);
            return result;
        }

        public override async Task<dynamic> QueryAsync(string path, string data)
        {
            var parameters = new dynamic[2];
            parameters[0] = path;
            parameters[1] = data;

            try
            {
                var result = await SendJsonRpc("query", parameters);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception($"Quering {path} failed: { e.Message}.");
            }
        }

        public override async Task<FinalExecutionOutcome> SendTransactionAsync(SignedTransaction signedTransaction)
        {
            var bytes = signedTransaction.ToByteArray();
            var parameters = new dynamic[1];
            parameters[0] = Convert.ToBase64String(bytes, 0, bytes.Length);
            var rawOutcomeResult = await SendJsonRpc("broadcast_tx_commit", parameters);
            var result = FinalExecutionOutcome.FromDynamicJsonObject(rawOutcomeResult);
            return result;
        }

        private async Task<dynamic> SendJsonRpc(string method, dynamic[] parameters)
        {
            dynamic request = new ExpandoObject();

            request.method = method;
            request.parameters = parameters;
            request.id = _id++;
            request.jsonrpc = "2.0";
            var requestString = JsonConvert.SerializeObject(request).Replace("\"parameters\":", "\"params\":");
            try
            {
                var result = await Web.FetchJsonAsync(_connection, requestString);
                return result;
            }
            catch (HttpException e)
            {
                throw new Exception($"{e.ErrorCode}: {e.Message}");
            }
        }
    }
}