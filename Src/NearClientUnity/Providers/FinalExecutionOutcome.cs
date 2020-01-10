namespace NearClientUnity.Providers
{
    public class FinalExecutionOutcome
    {
        public ExecutionOutcomeWithId[] Receipts { get; set; }
        public FinalExecutionStatus Status { get; set; }
        public FinalExecutionStatusBasic StatusBasic { get; set; }
        public ExecutionOutcomeWithId Transaction { get; set; }
    }
}