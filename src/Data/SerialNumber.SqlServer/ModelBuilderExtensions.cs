using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CnSharp.Data.SerialNumber.SqlServer;

public static class ModelBuilderExtensions
{
    public static void ConfigureSequence<TId, TRule, TRolling>(this ModelBuilder modelBuilder)
        where TRule : class, ISerialNumberRule<TId>
        where TRolling : class, ISerialNumberRolling<TId>
    {
        modelBuilder.Entity<TRule>(b =>
        {
            b.ToTable(typeof(TRule).Name).HasKey(m => m.Id);
            if (typeof(TId) == typeof(Guid))
            {
                b.Property(m => m.Id).HasDefaultValueSql("NEWID()");
            }

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
            if (typeof(TId) == typeof(Guid))
            {
                b.Property(m => m.Id).HasDefaultValueSql("NEWID()");
            }

            b.Property(m => m.Code).HasColumnType("varchar(32)").HasMaxLength(32)
                .IsRequired();
            b.Property(m => m.Date).HasColumnType("char(10)").IsRequired();
        });
    }

    public static void ConfigureSeedData<TId, TRule>(this ModelBuilder modelBuilder, List<TRule> data)
        where TRule : class, ISerialNumberRule<TId>
    {
        if (data == null || !data.Any())
        {
            return;
        }
        
        data.Where(m => string.IsNullOrEmpty(m.SequencePattern)).ToList().ForEach(m =>
            {
                modelBuilder.HasSequence<long>(m.SeperatedSequenceName)
                    .StartsAt(m.StartValue).IncrementsBy(m.Step);
            }
        );
        modelBuilder.Entity<TRule>().HasData(data);
    }
    
    public static void ConfigureCreationTimestamp(this PropertyBuilder propertyBuilder)
    {
        propertyBuilder.HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnAdd();
    }

    public static void ConfigureModificationTimestamp(this PropertyBuilder propertyBuilder)
    {
        propertyBuilder.HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnAddOrUpdate();
    }
}