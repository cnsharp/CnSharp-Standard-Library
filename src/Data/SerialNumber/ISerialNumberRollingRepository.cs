using System.Collections.Generic;
using System.Threading.Tasks;

namespace CnSharp.Data.SerialNumber
{
    public interface ISerialNumberRollingRepository<TId, in TRule> 
        where TRule : class, ISerialNumberRule<TId>
    {
        Task<long> GetSequenceValue(TRule rule, Dictionary<string, object> context);
    }
}
