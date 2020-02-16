using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.SqlServer
{
    public static class DbContextExtension
    {
        public static async Task<long> FetchSequenceValue(this DbContext context, string sequence)
        {
            var result = new SqlParameter("@result", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };

            await context.Database.ExecuteSqlRawAsync(
                $"SELECT @result = (NEXT VALUE FOR [dbo].[{sequence}])", result);

            return (long)result.Value;
        }
    }
}
