namespace NearClientUnity.Providers
{
    public class ExecutionStatus
    {
        public ExecutionError Failure { get; set; }
        public string SuccessReceiptId { get; set; }
        public string SuccessValue { get; set; }

        public static ExecutionStatus FromDynamicJsonObject(dynamic jsonObject)
        {
            if (jsonObject.ToString() == "Unknown")
            {
                return new ExecutionStatus();
            }

            var isFailure = jsonObject.Failure != null;

            if (isFailure)
            {
                return new ExecutionStatus()
                {
                    Failure = ExecutionError.FromDynamicJsonObject(jsonObject.Failure),
                };
            }

            return new ExecutionStatus()
            {
                SuccessReceiptId = jsonObject.SuccessReceiptId,
                SuccessValue = jsonObject.SuccessValue,
            };
        }
    }
}