namespace NearClientUnity.Providers
{
    public abstract class ExecutionOutcome
    {
        public abstract int GasBurnt { get; set; }
        public abstract string[] Logs { get; set; }
        public abstract string[] ReceiptIds { get; set; }
        public abstract ExecutionStatus Status { get; set; }
        public abstract ExecutionStatusBasic StatusBasic { get; set; }
    }
}