using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.MySql;

public interface ISequenceDbContext<TId, TRule, TRolling>
    where TRule : class, ISerialNumberRule<TId>
    where TRolling : class, ISerialNumberRolling<TId>
{
    List<TRule> SeedData { get; set; }
    DbSet<TRule> SerialNumberRules { get; set; }
    DbSet<TRolling> SerialNumberRollings { get; set; }

    DbContext DbContext { get; }

    string GetTableName(Type entityType);
}