using Newtonsoft.Json.Linq;

namespace NearClientUnity.Providers
{
    public class NodeStatusResult
    {
        public string ChainId { get; set; }
        public string RpcAddr { get; set; }
        public SyncInfo SyncInfo { get; set; }
        public JArray Validators { get; set; }

        public static NodeStatusResult FromDynamicJsonObject(dynamic jsonObject)
        {
            var result = new NodeStatusResult()
            {
                ChainId = jsonObject.chain_id,
                RpcAddr = jsonObject.rpc_addr,
                SyncInfo = new SyncInfo()
                {
                    LatestBlockHash = jsonObject.sync_info.latest_block_hash,
                    LatestBlockHeight = jsonObject.sync_info.latest_block_height,
                    LatestBlockTime = jsonObject.sync_info.latest_block_time,
                    LatestStateRoot = jsonObject.sync_info.latest_state_root,
                    Syncing = jsonObject.sync_info.syncing
                },
                Validators = jsonObject.validators
            };
            return result;
        }
    }
}