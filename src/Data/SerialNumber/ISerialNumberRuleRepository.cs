using System;
using System.Threading.Tasks;

namespace CnSharp.Data.SerialNumber
{
    public interface ISerialNumberRuleRepository
    {
        Task Add(SerialNumberRule rule);

        Task Modify(SerialNumberRule rule);

        Task<SerialNumberRule> Get(string code);
    }
}
