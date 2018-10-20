using System;
using Xunit;

namespace PSBase.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var x = "This is a test".GetSha1Hash();
        }
    }
}
