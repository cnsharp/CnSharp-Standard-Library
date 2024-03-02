using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace CnSharp.Data.SerialNumber.MySql
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
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.Id).HasColumnType("char(36)").HasValueGenerator<GuidValueGenerator>();
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.SequencePattern).HasColumnType("varchar(32)").HasMaxLength(32);
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.NumberPattern).HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SerialNumberRule>().Property(m => m.DateUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                .ValueGeneratedOnAddOrUpdate();
            if (SeedData != null)
            {
                modelBuilder.Entity<SerialNumberRule>().HasData(SeedData);
            }

            modelBuilder.Entity<SerialNumberRolling>().ToTable("SerialNumberRolling").HasKey(m => m.Id);
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Id).HasColumnType("char(36)").HasValueGenerator<GuidValueGenerator>();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Date).HasColumnType("char(10)").IsRequired();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.DateUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
                .ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<SerialNumberRolling>().Property(m => m.Version).IsRowVersion();
           
        }
    }
}