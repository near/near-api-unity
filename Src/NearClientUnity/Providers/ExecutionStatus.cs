namespace NearClientUnity.Providers
{
    public class ExecutionStatus
    {
        public ExecutionError Failure { get; set; }
        public string SuccessReceiptId { get; set; }
        public string SuccessValue { get; set; }
    }
}