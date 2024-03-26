using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.SqlServer
{
    public abstract class BaseSequenceDbContext<TId, TRule, TRolling>(DbContextOptions options)
        : DbContext(options), ISequenceDbContext<TId, TRule, TRolling>
        where TRule : class, ISerialNumberRule<TId>
        where TRolling : class, ISerialNumberRolling<TId>
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureSequence<TId, TRule, TRolling>();
            modelBuilder.ConfigureSeedData<TId, TRule>(SeedData);
        }

        public List<TRule> SeedData { get; set; }
        public DbSet<TRule> SerialNumberRules { get; set; }
        public DbSet<TRolling> SerialNumberRollings { get; set; }
        public DbContext DbContext => this;

        public string GetTableName(Type entityType)
        {
            return Model.FindEntityType(entityType)?.GetTableName() ??
                   throw new ArgumentException($"entity type {entityType} not found.");
        }
    }
}