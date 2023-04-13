using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagsManagement.Migrations
{
    /// <inheritdoc />
    public partial class modifiedBaseEntity01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Posts");
        }
    }
}
