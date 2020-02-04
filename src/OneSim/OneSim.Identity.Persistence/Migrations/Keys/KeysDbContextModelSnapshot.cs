﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OneSim.Identity.Persistence;

namespace OneSim.Identity.Persistence.Migrations.Keys
{
    [DbContext(typeof(KeysDbContext))]
    partial class KeysDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("OneSim.Identity.Domain.Entities.Key", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Data")
                        .HasColumnType("text");

                    b.Property<int>("Purpose")
                        .HasColumnType("integer");

                    b.Property<string>("SecurityKeyId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Keys");
                });
#pragma warning restore 612, 618
        }
    }
}
