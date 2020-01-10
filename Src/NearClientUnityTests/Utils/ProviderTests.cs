using System;
using NearClientUnity.Providers;
using NUnit.Framework;

namespace NearClientUnityTests.Utils
{
    [TestFixture]
    public class ProviderTests
    {
        [Test]
        public void ShouldHaveCorrectFinalResult()
        {
            var excStatus = new ExecutionStatus()
            {
                SuccessReceiptId = "11112"
            };
            var excOutcome = new ExecutionOutcome()
            {
                Status = excStatus,
                Logs = new string[0],
                ReceiptIds = new string[] { "11112" },
                GasBurnt = 1

            };
            var transaction = new ExecutionOutcomeWithId()
            {
                Id = "11111",
                Outcome = excOutcome
            };
            var firstExcStatus = new ExecutionStatus()
            {
                SuccessValue = "e30="
            };
            var firstExcOutcome = new ExecutionOutcome()
            {
                Status = firstExcStatus,
                Logs = new string[0],
                ReceiptIds = new string[] { "11112" },
                GasBurnt = 9001

            };
            var secondExcStatus = new ExecutionStatus()
            {
                SuccessValue = ""
            };
            var secondExcOutcome = new ExecutionOutcome()
            {
                Status = secondExcStatus,
                Logs = new string[0],
                ReceiptIds = new string[0],
                GasBurnt = 0

            };
            var receipts = new ExecutionOutcomeWithId[] {
                new ExecutionOutcomeWithId { Id = "11112", Outcome = firstExcOutcome },
                new ExecutionOutcomeWithId { Id = "11113", Outcome = secondExcOutcome }
            };
            var result = new FinalExecutionOutcome()
            {
                Status = new FinalExecutionStatus()
                {
                    SuccessValue = "e30="
                },
                Transaction = transaction,
                Receipts = receipts

            };
            dynamic lastResult = Provider.GetTransactionLastResult(result);            
            Assert.IsFalse(lastResult is null);
        }

        [Test]
        public void ShouldHaveCorrectFinalResultWithNull()
        {
            var excStatus = new ExecutionStatus()
            {
                SuccessReceiptId = "11112"
            };
            var excOutcome = new ExecutionOutcome()
            {
                Status = excStatus,
                Logs = new string[0],
                ReceiptIds = new string[] { "11112" },
                GasBurnt = 1

            };
            var transaction = new ExecutionOutcomeWithId()
            {
                Id = "11111",
                Outcome = excOutcome
            };
            var firstExcStatus = new ExecutionStatus()
            {
                Failure = new ExecutionError()
            };
            var firstExcOutcome = new ExecutionOutcome()
            {
                Status = firstExcStatus,
                Logs = new string[0],
                ReceiptIds = new string[] { "11112" },
                GasBurnt = 9001

            };
            var secondExcStatus = new ExecutionStatus()
            {
                SuccessValue = ""
            };
            var secondExcOutcome = new ExecutionOutcome()
            {
                Status = secondExcStatus,
                Logs = new string[0],
                ReceiptIds = new string[0],
                GasBurnt = 0

            };
            var receipts = new ExecutionOutcomeWithId[] {
                new ExecutionOutcomeWithId { Id = "11112", Outcome = firstExcOutcome },
                new ExecutionOutcomeWithId { Id = "11113", Outcome = secondExcOutcome }
            };
            var result = new FinalExecutionOutcome()
            {
                Status = new FinalExecutionStatus()
                {
                    Failure = new ExecutionError()
                },
                Transaction = transaction,
                Receipts = receipts

            };
            dynamic lastResult = Provider.GetTransactionLastResult(result);
            Assert.IsTrue(lastResult is null);
        }
    }
}
