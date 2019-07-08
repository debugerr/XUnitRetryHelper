namespace XunitRetryHelper.Lib
{
    using System;
    using System.Diagnostics;
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

                    var retryDelay = Math.Min(60_000, 5000 * runCount);
                    var msg = $"{DateTime.UtcNow.ToString("HH:mm:ss.fff")}, xunit will perform retry number {runCount} in {retryDelay}ms";
                    
                    //var diagMessage = new DiagnosticMessage(msg);
                    //diagnosticMessageSink.OnMessage(diagMessage);

                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine(msg);
                    }
                    else
                    {
                        Console.WriteLine(msg);
                    }
                    await Task.Delay(retryDelay);
                }
            }
        }
    }
}
