using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.SqlServer
{
    public class SerialNumberRollingRepository<TId, TRule, TRolling>(ISequenceDbContext<TId, TRule, TRolling> dbContext)
        : ISerialNumberRollingRepository<TId, TRule>
        where TRule : class, ISerialNumberRule<TId>
        where TRolling : class, ISerialNumberRolling<TId>, new()
    {
        private const string CreateOrResetSequenceSql = "IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = '{0}') CREATE SEQUENCE {0} START WITH {1} INCREMENT BY {2}; ELSE ALTER SEQUENCE [{0}] RESTART WITH {1};";

        public async Task<long> GetSequenceValue(TRule rule, Dictionary<string, object> context)
        {
            var seqPattern = string.IsNullOrWhiteSpace(rule.SequencePattern) ? rule.Code : rule.SequencePattern;
            var sequenceName = SerialNumberJoiner.GetSequenceName(seqPattern, context);
            var date = DateTime.Today.ToString("yyyy-MM-dd");
            var rolling = await dbContext.SerialNumberRollings.FirstOrDefaultAsync(m => m.Code == sequenceName && m.Date == date);
            bool isNew = false;
            if (rolling == null)
            {
                rolling = new TRolling
                {
                    Code = sequenceName,
                    Date = date,
                    CurrentValue = rule.StartValue
                };
                isNew = true;
                await dbContext.DbContext.Database.ExecuteSqlRawAsync(string.Format(CreateOrResetSequenceSql, sequenceName, rule.StartValue, rule.Step));
            }

            var number = await dbContext.DbContext.FetchSequenceValue(sequenceName);
            rolling.CurrentValue = number;
            if(isNew)
            {
                await dbContext.SerialNumberRollings.AddAsync(rolling);
            }
            await dbContext.DbContext.SaveChangesAsync();
            return number;
        }
    }
    
    public class SerialNumberRollingRepository(
        ISequenceDbContext<Guid, SerialNumberRule, SerialNumberRolling> dbContext)
        : SerialNumberRollingRepository<Guid, SerialNumberRule, SerialNumberRolling>(dbContext);
}