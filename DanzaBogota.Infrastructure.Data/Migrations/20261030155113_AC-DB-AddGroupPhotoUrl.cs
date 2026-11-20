using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanzaBogota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ACDBAddGroupPhotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RutUrl",
                schema: "GROUPS",
                table: "Groups",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                schema: "GROUPS",
                table: "Groups",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                schema: "EVENTS",
                table: "EventResources",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                schema: "GROUPS",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                schema: "EVENTS",
                table: "EventResources");

            migrationBuilder.AlterColumn<string>(
                name: "RutUrl",
                schema: "GROUPS",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
