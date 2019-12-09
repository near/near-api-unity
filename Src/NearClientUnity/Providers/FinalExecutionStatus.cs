using System;
namespace NearClientUnity.Providers
{
    public abstract class FinalExecutionStatus
    {
        public abstract string SuccessValue{ get; set; }
        public abstract ExecutionError Failure { get; set; }
    }
}