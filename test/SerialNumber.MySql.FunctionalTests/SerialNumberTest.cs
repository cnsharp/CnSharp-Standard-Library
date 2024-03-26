using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Xunit.Abstractions;

namespace CnSharp.Data.SerialNumber.MySql.FunctionalTests
{
    public class SerialNumberTest(ITestOutputHelper testOutputHelper)
    {
        TestSequenceDbContext ConnectTestSequenceDb()
        {
            var builder = new DbContextOptionsBuilder<TestSequenceDbContext>();
            builder.UseMySql("Server=localhost;Database=CSF;Uid=root;Pwd=RootRoot");
            return new TestSequenceDbContext(builder.Options);
        }


        [Fact]
        async Task TestGetNumber()
        {
            var num = await GetNumber();
            testOutputHelper.WriteLine(num);
            Assert.NotNull(num);
        }

        private async Task<string> GetNumber()
        {
            using (var dbContext = ConnectTestSequenceDb()) 
            {
                ISerialNumberGenerator gen = new SerialNumberGenerator(
                    new SerialNumberRuleRepository(dbContext), 
                    new SerialNumberRollingRepository(dbContext));
                var num = await gen.Next("PO", new Dictionary<string, object>
                {
                    {"wid", "BJ01"}
                });
                return num;
            }
        }

        [Fact]
        async Task TestMultiThread()
        {
            var tasks = Enumerable.Range(1, 50).Select(_ => GetNumber());
            tasks.AsParallel().ForAll(task => testOutputHelper.WriteLine(task.Result));
        }
    }

    public class TestSequenceDbContext : SequenceDbContext
    {
        public TestSequenceDbContext(DbContextOptions options) : base(options)
        {
            SeedData =
            [
                new SerialNumberRule
                {
                    Id = Guid.Parse("a99f370d-b4a9-4e27-8004-6cb6cf8bf89a"),
                    Code = "PO",
                    StartValue = 1,
                    Step = 1,
                    SequencePattern = "%wid%PO",
                    NumberPattern = "%wid%PO%yyyyMMdd%%06d%",
                    DateCreated = DateTimeOffset.MinValue
                }
            ];
        }
    }


    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestSequenceDbContext>
    {
        public TestSequenceDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TestSequenceDbContext>();
            builder.UseMySql("Server=localhost;Database=CSF;Uid=root;Pwd=RootRoot");
            return new TestSequenceDbContext(builder.Options);
        }
    }
}
