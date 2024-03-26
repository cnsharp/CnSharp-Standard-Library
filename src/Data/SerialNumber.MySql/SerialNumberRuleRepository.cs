using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.MySql
{
    public class SerialNumberRuleRepository<TId, TRule, TRolling>(
        ISequenceDbContext<TId, TRule, TRolling> dbContext)
        : ISerialNumberRuleRepository<TId, TRule>
        where TRule : class, ISerialNumberRule<TId>
        where TRolling : class, ISerialNumberRolling<TId>, new()
    {

        public async Task Add(TRule rule)
        {
            await dbContext.SerialNumberRules.AddAsync(rule);
            await dbContext.DbContext.SaveChangesAsync();
        }

        public async Task Modify(TRule rule)
        {
            var item = rule.Id != null
                ? await dbContext.SerialNumberRules.FindAsync(rule.Id)
                : await dbContext.SerialNumberRules.FirstOrDefaultAsync(m => m.Code == rule.Code);
            if (item == null)
            {
                throw new KeyNotFoundException(
                    $"rule {(rule.Id != null ? "id" : "code")} {(rule.Id != null ? rule.Id.ToString() : rule.Code)} not found.");
            }

            item.SequencePattern = rule.SequencePattern;
            item.NumberPattern = rule.NumberPattern;
            item.StartValue = rule.StartValue;
            item.Step = rule.Step;
            await dbContext.DbContext.SaveChangesAsync();
        }

        public async Task<TRule> Get(string code)
        {
            return await dbContext.SerialNumberRules.FirstOrDefaultAsync(m => m.Code == code);
        }
        
    }
    
    public class SerialNumberRuleRepository(
        ISequenceDbContext<Guid, SerialNumberRule, SerialNumberRolling> dbContext)
        : SerialNumberRuleRepository<Guid, SerialNumberRule, SerialNumberRolling>(dbContext);
}