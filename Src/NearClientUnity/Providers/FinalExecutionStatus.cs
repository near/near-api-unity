namespace NearClientUnity.Providers
{
    public abstract class FinalExecutionStatus
    {
        public abstract ExecutionError Failure { get; set; }
        public abstract string SuccessValue { get; set; }
    }
}