using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagsManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Videos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Tags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Tags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Posts");
        }
    }
}
