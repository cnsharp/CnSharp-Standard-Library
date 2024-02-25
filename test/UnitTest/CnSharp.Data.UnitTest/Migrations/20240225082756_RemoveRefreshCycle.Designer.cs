﻿// <auto-generated />
using System;
using CnSharp.Data.UnitTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CnSharp.Data.UnitTest.Migrations
{
    [DbContext(typeof(TestSequenceDbContext))]
    [Migration("20240225082756_RemoveRefreshCycle")]
    partial class RemoveRefreshCycle
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("Relational:Sequence:.PO", "'PO', '', '1', '1', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CnSharp.Data.SerialNumber.SerialNumberRolling", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(36)");

                    b.Property<string>("Code")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.Property<long>("CurrentValue")
                        .HasColumnType("bigint");

                    b.Property<string>("Date")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSDATETIMEOFFSET()");

                    b.Property<DateTimeOffset>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSDATETIMEOFFSET()");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("SerialNumberRollings");
                });

            modelBuilder.Entity("CnSharp.Data.SerialNumber.SerialNumberRule", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(36)");

                    b.Property<string>("Code")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSDATETIMEOFFSET()");

                    b.Property<DateTimeOffset>("DateUpdated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSDATETIMEOFFSET()");

                    b.Property<string>("Pattern")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.Property<int>("StartValue")
                        .HasColumnType("int");

                    b.Property<int>("Step")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SerialNumberRules");

                    b.HasData(
                        new
                        {
                            Id = "8794f5dd-79ae-4e06-aa6c-d6a80bc78ea7",
                            Code = "PO",
                            DateCreated = new DateTimeOffset(new DateTime(2024, 2, 25, 16, 27, 55, 912, DateTimeKind.Unspecified).AddTicks(3820), new TimeSpan(0, 8, 0, 0, 0)),
                            DateUpdated = new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Pattern = "%wid%PO%yyyyMMdd%%seq5%",
                            StartValue = 1,
                            Step = 1
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
