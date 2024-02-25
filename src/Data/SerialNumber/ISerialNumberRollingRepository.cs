using System.Collections.Generic;
using System.Threading.Tasks;

namespace CnSharp.Data.SerialNumber
{
    public interface ISerialNumberRollingRepository
    {
        Task<long> GetSequenceValue(SerialNumberRule rule, Dictionary<string, object> context);
    }
}
