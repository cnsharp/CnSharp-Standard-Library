using System.Collections.Generic;
using System.Threading.Tasks;

namespace CnSharp.Data.SerialNumber
{
    public interface ISerialNumberGenerator
    {
        Task<string> Next(string code, Dictionary<string, object> context = null);
    }
}
