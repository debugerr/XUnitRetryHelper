namespace XunitRetryHelper.Lib
{
    using Xunit;
    using Xunit.Sdk;

    [XunitTestCaseDiscoverer("XunitRetryHelper.Lib.RetryFactDiscoverer", "XunitRetryHelper.Lib")]
    public class RetryFactAttribute : FactAttribute
    {
        public RetryFactAttribute()
        {
        }

        public RetryFactAttribute(int maxRetries)
        {
            this.MaxRetries = maxRetries;
        }

        public int MaxRetries { get; set; } = 5;
    }
}
