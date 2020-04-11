using Microsoft.EntityFrameworkCore.Migrations;

namespace OneSim.Identity.Persistence.Migrations.Keys
{
    public partial class AddSecurityKeyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityKeyId",
                table: "Keys",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityKeyId",
                table: "Keys");
        }
    }
}
