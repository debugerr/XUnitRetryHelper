namespace XunitRetryHelper.Lib
{
    using Xunit;
    using Xunit.Sdk;

    [XunitTestCaseDiscoverer("XunitRetryHelper.Lib.RetryTheoryTestDiscoverer", "XunitRetryHelper.Lib")]
    public class RetryTheoryAttribute : TheoryAttribute
    {
        public RetryTheoryAttribute()
        {
        }

        public RetryTheoryAttribute(int maxRetries)
        {
            this.MaxRetries = maxRetries;
        }

        public int MaxRetries { get; set; } = 5;
    }
}
