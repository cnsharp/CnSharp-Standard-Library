using System;
using Microsoft.EntityFrameworkCore;

namespace CnSharp.Data.SerialNumber.SqlServer
{
    public class SequenceDbContext(DbContextOptions options)
        : BaseSequenceDbContext<Guid, SerialNumberRule, SerialNumberRolling>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SerialNumberRule>(b =>
            {
                b.Property(m => m.DateCreated).ConfigureCreationTimestamp();
                b.Property(m => m.DateUpdated).ConfigureModificationTimestamp();
            });

            modelBuilder.Entity<SerialNumberRolling>(b =>
            {
                b.Property(m => m.DateCreated).ConfigureCreationTimestamp();
                b.Property(m => m.DateUpdated).ConfigureModificationTimestamp();
            });
        }
    }
}