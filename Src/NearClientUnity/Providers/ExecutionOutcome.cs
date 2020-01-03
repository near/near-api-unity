namespace NearClientUnity.Providers
{
    public class ExecutionOutcome
    {
        public int GasBurnt { get; set; }
        public string[] Logs { get; set; }
        public string[] ReceiptIds { get; set; }
        public ExecutionStatus Status { get; set; }
        public ExecutionStatusBasic StatusBasic { get; set; }
    }
}