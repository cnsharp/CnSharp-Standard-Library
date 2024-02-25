using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CnSharp.Data.SerialNumber;
using CnSharp.Data.SerialNumber.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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

        TestSequenceDbContext ConnectTestSequenceDb()
        {
            var builder = new DbContextOptionsBuilder<TestSequenceDbContext>();
            builder.UseSqlServer("Server=.;Database=CSF;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new TestSequenceDbContext(builder.Options);
        }


        [Fact]
        async Task TestGetNumber()
        {
            var num = await GetNumber();
            _testOutputHelper.WriteLine(num);
            Assert.NotNull(num);
        }

        private async Task<string> GetNumber()
        {
            using (var dbContext = ConnectTestSequenceDb()) 
            {
                ISerialNumberGenerator gen = new SerialNumberGenerator(new SerialNumberRuleRepository(dbContext), new SerialNumberRollingRepository(dbContext));
                var num = await gen.Next("PO", new Dictionary<string, object>
                {
                    {"wid", "BJ01"}
                });
                return num;
            }
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


        [Fact]
        async Task TestMultiThread()
        {
            var tasks = Enumerable.Range(1, 500).Select(m => GetNumber());
            (await Task.WhenAll(tasks)).ToList().ForEach(m => _testOutputHelper.WriteLine(m));
        }
    }

    public class TestSequenceDbContext : SequenceDbContext
    {
        public TestSequenceDbContext(DbContextOptions options) : base(options)
        {
            base.SeedData = new List<SerialNumberRule>
            {
                new SerialNumberRule
                {
                    Id = Guid.NewGuid(),
                    Code = "PO",
                    StartValue = 1,
                    Step = 1,
                    SequencePattern = "%wid%PO",
                    NumberPattern = "%wid%PO%yyyyMMdd%%06d%"
                }
            };
        }
    }


    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestSequenceDbContext>
    {
        public TestSequenceDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TestSequenceDbContext>();
            builder.UseSqlServer("Server=.;Database=CSF;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new TestSequenceDbContext(builder.Options);
        }
    }
}
