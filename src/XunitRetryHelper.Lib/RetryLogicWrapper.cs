namespace XunitRetryHelper.Lib
{
    using System;
    using System.Diagnostics;
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
                                                XunitTestCase testCase,
                                                CancellationTokenSource cancellationTokenSource)
        {
            var testExecutionCount = 0;

            using (var delayedMessageBus = new DelayedMessageBus(messageBus))
            {
                while (true)
                {
                    // delayedMessageBus.StartRun();
                    var summary = await executer(diagnosticMessageSink, delayedMessageBus, constructorArguments, aggregator, cancellationTokenSource);

                    if (aggregator.HasExceptions || summary.Failed == 0 || ++testExecutionCount > maxRetries)
                    {
                        delayedMessageBus.Complete();
                        if (summary.Failed == 0 && testExecutionCount > 1)
                        {
                            LogMessage($"Test '{testCase.DisplayName}' succeeded after {testExecutionCount} executions.");
                        }

                        return summary;
                    }

                    var retryDelay = Math.Min(60_000, 5000 * testExecutionCount);
                    var msg = $"performing retry number {testExecutionCount} in {retryDelay}ms for Test '{testCase.DisplayName}'";
                    LogMessage(msg);

                    await Task.Delay(retryDelay, cancellationTokenSource.Token);
                }
            }
        }

        private static void LogMessage(string msg)
        {
            msg = $"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} [Test] {msg}";

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }
    }
}
