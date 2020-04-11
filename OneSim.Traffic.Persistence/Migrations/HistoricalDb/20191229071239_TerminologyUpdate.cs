using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OneSim.Traffic.Persistence.Migrations.HistoricalDb
{
    public partial class TerminologyUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusFiles");

            migrationBuilder.CreateTable(
                name: "TrafficData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateReceived = table.Column<DateTime>(nullable: false),
                    FetchTime = table.Column<TimeSpan>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    TrafficData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrafficData");

            migrationBuilder.CreateTable(
                name: "StatusFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateReceived = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DownloadTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SourceUrl = table.Column<string>(type: "text", nullable: true),
                    StatusFile = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusFiles", x => x.Id);
                });
        }
    }
}
