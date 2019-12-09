namespace NearClientUnity.Providers
{
    public abstract class ExecutionOutcomeWithId
    {
        public abstract string Id { get; set; }
        public abstract ExecutionOutcome Outcome { get; set; }
    }
}