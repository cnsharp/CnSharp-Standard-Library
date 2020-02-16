using System;
using System.Collections.Generic;
using System.Linq;
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
                    $"rule {(rule.Id != null ? "id" : "code")} {rule.Id ?? rule.Code} not found.");
            }

            item.Pattern = rule.Pattern;
            item.RefreshCycle = rule.RefreshCycle;
            item.StartValue = rule.StartValue;
            item.Step = rule.Step;
            await _dbContext.SaveChangesAsync();
        }

        private const string ResetSequenceSql = "ALTER SEQUENCE [dbo].[{0}] RESTART WITH {1};";

        public async Task<Tuple<string, long>> GetSequenceValue(string code)
        {
            var rule = await _dbContext.SerialNumberRules.FirstOrDefaultAsync(m => m.Code == code);
            if (rule == null)
            {
                throw new KeyNotFoundException($"rule code {code} not found.");
            }

            var part = GetDatePart(rule.RefreshCycle);
            var rolling = await _dbContext.SerialNumberRollings.FirstOrDefaultAsync(m =>
                m.Code == code && m.Y == part.Year && m.M == part.Month && m.D == part.Day);
            if(rolling == null)
            {
                rolling = new SerialNumberRolling
                {
                    Code = code,
                    Y = part.Year,
                    M = part.Month,
                    D = part.Day,
                    CurrentValue = rule.StartValue
                };
                await _dbContext.SerialNumberRollings.AddAsync(rolling);
                await _dbContext.Database.ExecuteSqlRawAsync(string.Format(ResetSequenceSql, code, rule.StartValue));
                await _dbContext.SaveChangesAsync();
                return new Tuple<string, long>(rule.Pattern, rule.StartValue);
            }

            var number = await  _dbContext.FetchSequenceValue(code);
            rolling.CurrentValue = number;
            await _dbContext.SaveChangesAsync();
            return new Tuple<string, long>(rule.Pattern, number);
        }


        private static DatePart GetDatePart(RefreshCycle cycle)
        {
            var part = new DatePart();
            part.Year = DateTime.Today.Year;
            switch (cycle)
            {
                case RefreshCycle.Daily:
                    part.Month = DateTime.Today.Month;
                    part.Day = DateTime.Today.Day;
                    break;
                case RefreshCycle.Monthly:
                    part.Month = DateTime.Today.Month;
                    break;
                case RefreshCycle.Never:
                    part.Year = 0;
                    break;
            }
            return part;
        }
    }
}