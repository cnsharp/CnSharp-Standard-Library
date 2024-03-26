using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CnSharp.Data.SerialNumber
{
    public class SerialNumberGenerator<TId,TRule>  : ISerialNumberGenerator where TRule : class, ISerialNumberRule<TId>
    {
        private readonly ISerialNumberRuleRepository<TId,TRule> _ruleRepository;
        private readonly ISerialNumberRollingRepository<TId,TRule> _rollingRepository;

        public SerialNumberGenerator(ISerialNumberRuleRepository<TId,TRule> ruleRepository, ISerialNumberRollingRepository<TId,TRule> rollingRepository)
        {
            _ruleRepository = ruleRepository;
            _rollingRepository = rollingRepository;
        }

        public async Task<string> Next(string code, Dictionary<string, object> context = null)
        {
            var rule = await _ruleRepository.Get(code);
            if (rule == null)
            {
                throw new KeyNotFoundException($"rule {code} not found.");
            }
            var number = await _rollingRepository.GetSequenceValue(rule, context);
            return SerialNumberJoiner.GetNumber(rule.NumberPattern, number, context);
        }
    }

    public class SerialNumberGenerator : SerialNumberGenerator<Guid, SerialNumberRule>
    {
        public SerialNumberGenerator(ISerialNumberRuleRepository<Guid, SerialNumberRule> ruleRepository, ISerialNumberRollingRepository<Guid, SerialNumberRule> rollingRepository) : base(ruleRepository, rollingRepository)
        {
        }
    }
}
