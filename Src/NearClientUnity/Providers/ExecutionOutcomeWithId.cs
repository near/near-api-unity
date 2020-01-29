namespace NearClientUnity.Providers
{
    public class ExecutionOutcomeWithId
    {
        public string Id { get; set; }
        public ExecutionOutcome Outcome { get; set; }

        public static ExecutionOutcomeWithId FromDynamicJsonObject(dynamic jsonObject)
        {
            var result = new ExecutionOutcomeWithId()
            {
                Id = jsonObject.id,
                Outcome = ExecutionOutcome.FromDynamicJsonObject(jsonObject.outcome)
            };
            return result;
        }
    }
}