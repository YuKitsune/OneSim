using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OneSim.Traffic.Persistence.Migrations.TrafficDb
{
    public partial class CreateStatusSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Controllers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NetworkId = table.Column<string>(nullable: true),
                    Callsign = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Server = table.Column<string>(nullable: true),
                    ProtocolRevision = table.Column<string>(nullable: true),
                    LogonTime = table.Column<DateTime>(nullable: false),
                    Frequency = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    FacilityType = table.Column<int>(nullable: false),
                    VisibilityRange = table.Column<int>(nullable: false),
                    Atis = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controllers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightPlans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FlightRules = table.Column<int>(nullable: false),
                    AircraftType = table.Column<string>(nullable: true),
                    TrueAirSpeed = table.Column<string>(nullable: true),
                    Altitude = table.Column<int>(nullable: false),
                    DepartureIcao = table.Column<string>(nullable: true),
                    ArrivalIcao = table.Column<string>(nullable: true),
                    AlternateIcao = table.Column<string>(nullable: true),
                    EstimatedTimeOfDeparture = table.Column<DateTime>(nullable: true),
                    ActualTimeOfDeparture = table.Column<DateTime>(nullable: true),
                    TimeEnroute = table.Column<TimeSpan>(nullable: true),
                    FuelOnBoard = table.Column<TimeSpan>(nullable: true),
                    Route = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    NetworkIdentifier = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Callsign = table.Column<string>(nullable: true),
                    NetworkId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    FlightPlanId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightNotifications_FlightPlans_FlightPlanId",
                        column: x => x.FlightPlanId,
                        principalTable: "FlightPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NetworkId = table.Column<string>(nullable: true),
                    Callsign = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Server = table.Column<string>(nullable: true),
                    ProtocolRevision = table.Column<string>(nullable: true),
                    LogonTime = table.Column<DateTime>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Altitude = table.Column<int>(nullable: false),
                    GroundSpeed = table.Column<int>(nullable: false),
                    Heading = table.Column<int>(nullable: false),
                    Squawk = table.Column<string>(nullable: false),
                    FlightPlanId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pilots_FlightPlans_FlightPlanId",
                        column: x => x.FlightPlanId,
                        principalTable: "FlightPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Altitude = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    PilotId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Points_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightNotifications_FlightPlanId",
                table: "FlightNotifications",
                column: "FlightPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Pilots_FlightPlanId",
                table: "Pilots",
                column: "FlightPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Points_PilotId",
                table: "Points",
                column: "PilotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Controllers");

            migrationBuilder.DropTable(
                name: "FlightNotifications");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "Pilots");

            migrationBuilder.DropTable(
                name: "FlightPlans");
        }
    }
}
