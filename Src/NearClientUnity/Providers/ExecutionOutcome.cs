using System.Collections.Generic;

namespace NearClientUnity.Providers
{
    public class ExecutionOutcome
    {
        public int GasBurnt { get; set; }
        public string[] Logs { get; set; }
        public string[] ReceiptIds { get; set; }
        public ExecutionStatus Status { get; set; }
        public ExecutionStatusBasic StatusBasic { get; set; }

        public static ExecutionOutcome FromDynamicJsonObject(dynamic jsonObject)
        {
            var logs = new List<string>();
            foreach (var log in jsonObject.logs)
            {
                logs.Add((string)log);
            }
            var receiptIds = new List<string>();
            foreach (var receipt in jsonObject.receipt_ids)
            {
                receiptIds.Add((string)receipt);
            }
            var result = new ExecutionOutcome()
            {
                GasBurnt = jsonObject.gas_burnt,
                Logs = logs.ToArray(),
                ReceiptIds = receiptIds.ToArray(),
                Status = ExecutionStatus.FromDynamicJsonObject(jsonObject.status),
            };
            return result;
        }
    }
}