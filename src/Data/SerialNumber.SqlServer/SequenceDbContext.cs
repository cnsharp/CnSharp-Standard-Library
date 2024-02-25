using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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


            SeedData?.Where(m => string.IsNullOrEmpty(m.SequencePattern)).ToList().ForEach(m =>
                {
                    modelBuilder.HasSequence<long>(m.Code)
                        .StartsAt(m.StartValue).IncrementsBy(m.Step);
                }
            );

            modelBuilder.Entity<SerialNumberRule>().ToTable("SerialNumberRule").HasKey(m => m.Id);
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.Id).HasDefaultValueSql("NEWID()"); 
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32);
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.SequencePattern).HasColumnType("varchar(32)").HasMaxLength(32);
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.NumberPattern).HasColumnType("varchar(32)").HasMaxLength(32);
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.DateCreated).HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.DateUpdated).HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();
            if (SeedData != null)
            {
                modelBuilder.Entity<SerialNumberRule>().HasData(SeedData);
            }

            modelBuilder.Entity<SerialNumberRolling>().ToTable("SerialNumberRolling").HasKey(m => m.Id);
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32);
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Date).HasColumnType("char(10)");
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.DateCreated).HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.DateUpdated).HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Version).IsRowVersion();
           
        }
    }

}