using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanzaBogota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRutAndRequiresRut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RutUrl",
                schema: "GROUPS",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseDirectorRut",
                schema: "GROUPS",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresRut",
                schema: "EVENTS",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RutUrl",
                schema: "GROUPS",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "UseDirectorRut",
                schema: "GROUPS",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "RequiresRut",
                schema: "EVENTS",
                table: "Events");
        }
    }
}
