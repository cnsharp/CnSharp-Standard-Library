using System.Collections.Generic;
using System.Threading.Tasks;

namespace CnSharp.Data.SerialNumber
{
    public class SerialNumberGenerator : ISerialNumberGenerator
    {
        private readonly ISerialNumberRuleRepository _ruleRepository;
        private readonly ISerialNumberRollingRepository _rollingRepository;

        public SerialNumberGenerator(ISerialNumberRuleRepository ruleRepository, ISerialNumberRollingRepository rollingRepository)
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
}
