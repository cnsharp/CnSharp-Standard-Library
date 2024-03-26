using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace CnSharp.Data.SerialNumber.MySql;

public static class ModelBuilderExtensions
{
    public static void ConfigureSequence<TId, TRule, TRolling>(this ModelBuilder modelBuilder)
        where TRule : class, ISerialNumberRule<TId>
        where TRolling : class, ISerialNumberRolling<TId>
    {
        modelBuilder.Entity<TRule>(b =>
        {
            b.ToTable(typeof(TRule).Name).HasKey(m => m.Id);
            b.Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32)
                .IsRequired();
            b.Property(m => m.SequencePattern).HasColumnType("varchar(32)")
                .HasMaxLength(32);
            b.Property(m => m.NumberPattern).HasColumnType("varchar(32)")
                .HasMaxLength(32).IsRequired();
        });


        modelBuilder.Entity<TRolling>(b =>
        {
            b.ToTable(typeof(TRolling).Name).HasKey(m => m.Id);
            b.Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32)
                .IsRequired();
            b.Property(m => m.Date).HasColumnType("char(10)").IsRequired();
        });
    }

    public static void ConfigureCreationTimestamp(this PropertyBuilder propertyBuilder)
    {
        propertyBuilder.HasDefaultValueSql("CURRENT_TIMESTAMP(6)").ValueGeneratedOnAdd();
    }

    public static void ConfigureModificationTimestamp(this PropertyBuilder propertyBuilder)
    {
        propertyBuilder.HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAddOrUpdate();
    }

    public static void ConfigureGuid(this PropertyBuilder propertyBuilder)
    {
        propertyBuilder.HasColumnType("char(36)");
    }

    public static void ConfigureGuidGenerator(this PropertyBuilder propertyBuilder)
    {
        propertyBuilder.HasColumnType("char(36)").HasValueGenerator<GuidValueGenerator>();
    }

    public static void ConfigureGuidKey(this EntityTypeBuilder b)
    {
        b.Property("Id").ConfigureGuidGenerator();
    }


    public static void ConfigureSeedData<TId, TRule>(this ModelBuilder modelBuilder, List<TRule> data)
        where TRule : class, ISerialNumberRule<TId>
    {
        if (!data.Any())
        {
            return;
        }

        modelBuilder.Entity<TRule>().HasData(data);
    }

}