namespace NearClientUnity.Providers
{
    public class FinalExecutionStatus
    {
        public ExecutionError Failure { get; set; }
        public string SuccessValue { get; set; }

        public static FinalExecutionStatus FromDynamicJsonObject(dynamic jsonObject)
        {
            var isFailure = jsonObject.Failure != null;
            if (isFailure)
            {
                return new FinalExecutionStatus()
                {
                    Failure = ExecutionError.FromDynamicJsonObject(jsonObject.Failure),
                };
            }
            return new FinalExecutionStatus()
            {
                SuccessValue = jsonObject.SuccessValue
            };
        }
    }
}