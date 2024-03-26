using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.MySql
{
    public class
        SerialNumberRollingRepository<TId, TRule, TRolling>(
            ISequenceDbContext<TId, TRule, TRolling> dbContext)
        : ISerialNumberRollingRepository<TId, TRule>
        where TRule : class, ISerialNumberRule<TId>
        where TRolling : class, ISerialNumberRolling<TId>, new()
    {
        public async Task<long> GetSequenceValue(TRule rule, Dictionary<string, object> context)
        {
            var seqPattern = string.IsNullOrWhiteSpace(rule.SequencePattern) ? rule.Code : rule.SequencePattern;
            var sequenceName = SerialNumberJoiner.GetSequenceName(seqPattern, context);
            var date = DateTime.Today.ToString("yyyy-MM-dd");
            await using (var transaction =
                         await dbContext.DbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel
                             .Serializable))
            {
                var rolling = dbContext.SerialNumberRollings.FromSqlRaw(
                    "select * from " + dbContext.GetTableName(typeof(TRolling)) +
                    " where Code={0} and Date={1} for update",
                    sequenceName,
                    date
                ).FirstOrDefault();
                if (rolling == null)
                {
                    rolling = new TRolling
                    {
                        Code = sequenceName,
                        Date = date,
                        CurrentValue = rule.StartValue
                    };
                    await dbContext.SerialNumberRollings.AddAsync(rolling);
                }
                else
                {
                    rolling.CurrentValue += rule.Step;
                }

                await dbContext.DbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return rolling.CurrentValue;
            }
        }
    }

    public class SerialNumberRollingRepository(
        ISequenceDbContext<Guid, SerialNumberRule, SerialNumberRolling> dbContext)
        : SerialNumberRollingRepository<Guid, SerialNumberRule, SerialNumberRolling>(dbContext);
    
}