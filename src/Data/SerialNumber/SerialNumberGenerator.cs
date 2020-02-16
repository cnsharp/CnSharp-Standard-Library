using System.Collections.Generic;
using System.Threading.Tasks;

namespace CnSharp.Data.SerialNumber
{
    public class SerialNumberGenerator : ISerialNumberGenerator
    {
        private readonly ISerialNumberRuleRepository _ruleRepository;

        public SerialNumberGenerator(ISerialNumberRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public async Task<string> Next(string code, Dictionary<string, object> context = null)
        {
            var rule = await _ruleRepository.GetSequenceValue(code);
            return SerialNumberJoiner.GetNumber(rule.Item1, rule.Item2, context);
        }
    }
}
