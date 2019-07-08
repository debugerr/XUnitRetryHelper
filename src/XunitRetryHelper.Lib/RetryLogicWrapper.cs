namespace XunitRetryHelper.Lib
{
    using System;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public static class RetryLogicWrapper
    {
        internal static async Task<RunSummary> PerformRetry(
                                                Func<IMessageSink,
                                                IMessageBus,
                                                object[],
                                                ExceptionAggregator,
                                                CancellationTokenSource, Task<RunSummary>> executer,
                                                int maxRetries,
                                                IMessageSink diagnosticMessageSink,
                                                IMessageBus messageBus,
                                                object[] constructorArguments,
                                                ExceptionAggregator aggregator,
                                                CancellationTokenSource cancellationTokenSource)
        {
            var runCount = 0;

            using (var delayedMessageBus = new DelayedMessageBus(messageBus))
            {
                while (true)
                {
                    var summary = await executer(diagnosticMessageSink, delayedMessageBus, constructorArguments, aggregator, cancellationTokenSource);
                    if (aggregator.HasExceptions || summary.Failed == 0 || ++runCount > maxRetries)
                    {
                        delayedMessageBus.Complete();
                        return summary;
                    }

                    var msg = string.Format("Execution failed (attempt #{0}), retrying...", runCount);
                    var diagMessage = new DiagnosticMessage(msg);
                    delayedMessageBus.QueueMessage(diagMessage);
                    diagnosticMessageSink.OnMessage(diagMessage);
                    Console.WriteLine($"***Retry: {msg}");

                    await Task.Delay(Math.Min(60_000, 5000 * runCount));
                }
            }
        }
    }
}
