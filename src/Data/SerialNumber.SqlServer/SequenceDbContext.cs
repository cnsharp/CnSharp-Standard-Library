using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace CnSharp.Data.SerialNumber.SqlServer
{
    public  class SequenceDbContext : DbContext 
    {
        public SequenceDbContext(DbContextOptions options) : base(options)  
        {
            
        }

        public List<SerialNumberRule> SeedData { get;set; }
        public DbSet<SerialNumberRule> SerialNumberRules { get; set; }
        public DbSet<SerialNumberRolling> SerialNumberRollings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SeedData?.ForEach(m =>
                {
                    modelBuilder.HasSequence<long>(m.Code)
                        .StartsAt(m.StartValue).IncrementsBy(m.Step);
                }
            );

            modelBuilder.Entity<SerialNumberRule>().HasKey(m => m.Id);
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.Id).HasColumnType("varchar(36)").HasValueGenerator<StringGuidValueGenerator>();
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32);
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.Pattern).HasColumnType("varchar(32)").HasMaxLength(32);
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.DateCreated).HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.DateUpdated).HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAddOrUpdate();
            if (SeedData != null)
            {
                modelBuilder.Entity<SerialNumberRule>().HasData(SeedData);
            }

            modelBuilder.Entity<SerialNumberRolling>().HasKey(m => m.Id);
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Id).HasColumnType("varchar(36)").HasValueGenerator<StringGuidValueGenerator>();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32);
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.DateCreated).HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.DateUpdated).HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Version).IsRowVersion();
           
        }
    }

    public class StringGuidValueGenerator : ValueGenerator<string> 
    {
        public override string Next(EntityEntry entry)
        {
            return Guid.NewGuid().ToString();
        }

        public override bool GeneratesTemporaryValues { get; } = false;
    }

    public class SequenceDbContextOptions<T> : DbContextOptions<T> where T : DbContext
    {
        public SequenceDbContextOptions()
        {
            
        }

        public SequenceDbContextOptions(DbContextOptions<T> options) : base()
        {
            
        }
        public List<SerialNumberRule> SerialNumberRules { get; set; }
    }


}