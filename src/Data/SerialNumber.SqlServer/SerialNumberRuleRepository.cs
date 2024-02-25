using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.SqlServer
{
    public class SerialNumberRuleRepository : ISerialNumberRuleRepository
    {
        private readonly SequenceDbContext _dbContext;

        public SerialNumberRuleRepository(SequenceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(SerialNumberRule rule)
        {
            await _dbContext.SerialNumberRules.AddAsync(rule);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Modify(SerialNumberRule rule)
        {
            var item = rule.Id != null
                ? await _dbContext.SerialNumberRules.FindAsync(rule.Id)
                : await _dbContext.SerialNumberRules.FirstOrDefaultAsync(m => m.Code == rule.Code);
            if (item == null)
            {
                throw new KeyNotFoundException(
                    $"rule {(rule.Id != null ? "id" : "code")} {(rule.Id != null ? rule.Id.ToString() : rule.Code)} not found.");
            }

            item.SequencePattern = rule.SequencePattern;
            item.NumberPattern = rule.NumberPattern;
            item.StartValue = rule.StartValue;
            item.Step = rule.Step;
            item.DateUpdated = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SerialNumberRule> Get(string code)
        {
            return await _dbContext.SerialNumberRules.FirstOrDefaultAsync(m => m.Code == code);
        }
    }
}