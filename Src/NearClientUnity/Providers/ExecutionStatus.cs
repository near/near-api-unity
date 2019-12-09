namespace NearClientUnity.Providers
{
    public abstract class ExecutionStatus
    {
        public abstract string SuccessValue { get; set; }
        public abstract string SuccessReceiptId { get; set; }
        public abstract ExecutionError Failure { get; set; }
    }
}