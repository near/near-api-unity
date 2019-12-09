namespace NearClientUnity.Providers
{
    public abstract class ExecutionError
    {
        public abstract string ErrorMessage { get; set; }
        public abstract string ErrorType { get; set; }
    }
}