using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.SqlServer
{
    public class SerialNumberRollingRepository : ISerialNumberRollingRepository
    {
        private readonly SequenceDbContext _dbContext;

        public SerialNumberRollingRepository(SequenceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private const string CreateOrResetSequenceSql = "IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = '{0}') CREATE SEQUENCE {0} START WITH {1} INCREMENT BY {2}; ELSE ALTER SEQUENCE [{0}] RESTART WITH {1};";

        public async Task<long> GetSequenceValue(SerialNumberRule rule, Dictionary<string, object> context)
        {
            var seqPattern = string.IsNullOrWhiteSpace(rule.SequencePattern) ? rule.Code : rule.SequencePattern;
            var sequenceName = SerialNumberJoiner.GetSequenceName(seqPattern, context);
            var date = DateTime.Today.ToString("yyyy-MM-dd");
            var rolling = await _dbContext.SerialNumberRollings.FirstOrDefaultAsync(m => m.Code == sequenceName && m.Date == date);
            bool isNew = false;
            if (rolling == null)
            {
                rolling = new SerialNumberRolling
                {
                    Code = sequenceName,
                    Date = date,
                    CurrentValue = rule.StartValue
                };
                isNew = true;
                await _dbContext.Database.ExecuteSqlRawAsync(string.Format(CreateOrResetSequenceSql, sequenceName, rule.StartValue, rule.Step));
            }

            var number = await _dbContext.FetchSequenceValue(sequenceName);
            rolling.CurrentValue = number;
            if(isNew)
            {
                await _dbContext.SerialNumberRollings.AddAsync(rolling);
            }
            else
            {
                rolling.DateUpdated = DateTimeOffset.Now;
            }
            await _dbContext.SaveChangesAsync();
            return number;
        }
    }
}