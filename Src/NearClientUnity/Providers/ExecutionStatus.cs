namespace NearClientUnity.Providers
{
    public abstract class ExecutionStatus
    {
        public abstract ExecutionError Failure { get; set; }
        public abstract string SuccessReceiptId { get; set; }
        public abstract string SuccessValue { get; set; }
    }
}