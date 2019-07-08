using System;
using Xunit;
using XunitRetryHelper.Lib;

namespace XunitRetryHelper.Tests
{
    public class XunitRetryHelperTests
    {
        private static int runsA;
        private static int runsB;

        //[RetryTheory]
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void AllPass_Theory(int n)
        {
            Assert.True(n > 0);
        }

        //[RetryTheory]
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void OneFail_Theory(int n)
        {
            Assert.True(n == 2 || runsA++ >= 1);
        }

        //[RetryTheory]
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void TwoFail_Theory(int n)
        {
            Assert.True(n == 2 || runsB++ >= 2);
        }
    }
}
