using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.MySql
{
    public class SerialNumberRollingRepository : ISerialNumberRollingRepository
    {
        private readonly SequenceDbContext _dbContext;

        public SerialNumberRollingRepository(SequenceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private const string CreateOrResetSequenceSql =
            "IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = '{0}') CREATE SEQUENCE {0} START WITH {1} INCREMENT BY {2}; ELSE ALTER SEQUENCE [{0}] RESTART WITH {1};";

        public async Task<long> GetSequenceValue(SerialNumberRule rule, Dictionary<string, object> context)
        {
            var seqPattern = string.IsNullOrWhiteSpace(rule.SequencePattern) ? rule.Code : rule.SequencePattern;
            var sequenceName = SerialNumberJoiner.GetSequenceName(seqPattern, context);
            var date = DateTime.Today.ToString("yyyy-MM-dd");
            await using (var transaction =
                         await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
            {
                var rolling = _dbContext.SerialNumberRollings.FromSqlInterpolated(
                    $"select * from SerialNumberRolling where Code={sequenceName} and Date={date} for update"
                ).FirstOrDefault();
                bool isNew = false;
                if (rolling == null)
                {
                    rolling = new SerialNumberRolling
                    {
                        Code = sequenceName,
                        Date = date,
                        CurrentValue = rule.StartValue
                    };
                    await _dbContext.SerialNumberRollings.AddAsync(rolling);
                }
                else
                {
                    rolling.CurrentValue += rule.Step;
                }
                
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return rolling.CurrentValue;
            }
        }
    }
}