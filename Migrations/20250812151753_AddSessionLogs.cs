using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZocoApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "SessionLogs",
                newName: "FechaInicio");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "SessionLogs",
                newName: "FechaFin");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "SessionLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "SessionLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "SessionLogs");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "SessionLogs");

            migrationBuilder.RenameColumn(
                name: "FechaInicio",
                table: "SessionLogs",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "FechaFin",
                table: "SessionLogs",
                newName: "EndTime");
        }
    }
}
