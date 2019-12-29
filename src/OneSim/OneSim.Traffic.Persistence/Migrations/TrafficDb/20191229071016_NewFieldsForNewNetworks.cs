using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OneSim.Traffic.Persistence.Migrations.TrafficDb
{
    public partial class NewFieldsForNewNetworks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtocolRevision",
                table: "Pilots");

            migrationBuilder.DropColumn(
                name: "EstimatedTimeOfDeparture",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "FuelOnBoard",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "TimeEnroute",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "ProtocolRevision",
                table: "Controllers");

            migrationBuilder.AddColumn<int>(
                name: "AdministrativeRating",
                table: "Pilots",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Endurance",
                table: "FlightPlans",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EstimatedEnrouteTime",
                table: "FlightPlans",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDepartureTime",
                table: "FlightPlans",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdministrativeRating",
                table: "Controllers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministrativeRating",
                table: "Pilots");

            migrationBuilder.DropColumn(
                name: "Endurance",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "EstimatedEnrouteTime",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "ScheduledDepartureTime",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "AdministrativeRating",
                table: "Controllers");

            migrationBuilder.AddColumn<string>(
                name: "ProtocolRevision",
                table: "Pilots",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedTimeOfDeparture",
                table: "FlightPlans",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FuelOnBoard",
                table: "FlightPlans",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeEnroute",
                table: "FlightPlans",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProtocolRevision",
                table: "Controllers",
                type: "text",
                nullable: true);
        }
    }
}
