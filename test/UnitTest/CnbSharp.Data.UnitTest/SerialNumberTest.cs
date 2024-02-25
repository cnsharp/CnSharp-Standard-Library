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

        ISerialNumberRuleRepository GetSerialNumberRuleRepo()
        {
            var builder = new DbContextOptionsBuilder<TestSequenceDbContext>();
            builder.UseSqlServer("Server=.;Database=CSF;Trusted_Connection=True;MultipleActiveResultSets=true");
            var context = new TestSequenceDbContext(builder.Options);
            //context.Database.Migrate();
            return new SerialNumberRuleRepository(context);
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
            ISerialNumberGenerator gen = new SerialNumberGenerator(GetSerialNumberRuleRepo());
            var num = await gen.Next("PO", new Dictionary<string, object>
            {
                {"wid", "BJ01"}
            });
            return num;
        }

        [Fact]
        void TestGenSn()
        {
            var sn = SerialNumberJoiner.GetNumber("%wid%PO%yyyyMMdd%%06d%", 1, new Dictionary<string, object>
            {
                {"wid", "BJ01"}
            });
            _testOutputHelper.WriteLine(sn);
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
                    Id = Guid.NewGuid().ToString(),
                    Code = "PO",
                    StartValue = 1,
                    Step = 1,
                    Pattern = "%wid%PO%yyyyMMdd%%seq5%"
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
