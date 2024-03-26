using System;
using System.Threading.Tasks;

namespace CnSharp.Data.SerialNumber
{
    public interface ISerialNumberRuleRepository<TId,TRule> 
        where TRule : class, ISerialNumberRule<TId>
    {
        Task Add(TRule rule);

        Task Modify(TRule rule);

        Task<TRule> Get(string code);
    }
}
