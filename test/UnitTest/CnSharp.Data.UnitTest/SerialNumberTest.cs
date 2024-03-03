using System;
using System.Collections.Generic;
using CnSharp.Data.SerialNumber;
using Xunit;
using Xunit.Abstractions;

namespace CnSharp.Data.UnitTest
{
    public class SerialNumberTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SerialNumberTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        void TestGenSn()
        {
            var sn = SerialNumberJoiner.GetNumber("%wid%PO%yyyyMMdd%%06d%", 1, new Dictionary<string, object>
            {
                {"wid", "BJ01"}
            });
            var expected = $"BJ01PO{DateTime.Today.ToString("yyyyMMdd")}000001";
            // _testOutputHelper.WriteLine(sn);
            Assert.Equal(expected, sn);
        }
    }
}