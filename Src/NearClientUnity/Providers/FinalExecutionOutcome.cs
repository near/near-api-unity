namespace NearClientUnity.Providers
{
    public abstract class FinalExecutionOutcome
    {
        public abstract FinalExecutionStatus Status { get; set; }
        public abstract FinalExecutionStatusBasic StatusBasic { get; set; }
        public abstract ExecutionOutcomeWithId Transaction { get; set; }
        public abstract ExecutionOutcomeWithId[] Receipts { get; set; }
    }
}