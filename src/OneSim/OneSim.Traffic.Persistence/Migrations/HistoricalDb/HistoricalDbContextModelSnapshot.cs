﻿// <auto-generated />

namespace OneSim.Traffic.Persistence.Migrations.HistoricalDb
{
    using System;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

    using HistoricalDbContext = OneSim.Traffic.Persistence.HistoricalDbContext;

    [DbContext(typeof(HistoricalDbContext))]
    partial class HistoricalDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("OneSim.Traffic.Domain.Entities.TrafficDataArchiveEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateReceived")
                        .HasColumnType("timestamp without time zone");

                    b.Property<TimeSpan>("FetchTime")
                        .HasColumnType("interval");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<string>("TrafficData")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TrafficData");
                });
#pragma warning restore 612, 618
        }
    }
}
