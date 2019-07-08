namespace XunitRetryHelper.Lib
{
    using System;
    using System.Collections.Generic;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class RetryTheoryTestDiscoverer : IXunitTestCaseDiscoverer
    {
        readonly IMessageSink diagnosticMessageSink;

        public RetryTheoryTestDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        {
            var maxRetries = Math.Max(1, theoryAttribute.GetNamedArgument<int>("MaxRetries"));
            yield return new RetryTheoryTestCase(this.diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, maxRetries);
        }
    }
}
