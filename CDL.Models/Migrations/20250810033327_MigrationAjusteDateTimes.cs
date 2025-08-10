using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDL.Models.Migrations
{
    /// <inheritdoc />
    public partial class MigrationAjusteDateTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserFields",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserFields",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "User",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserFields");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserFields");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "User");
        }
    }
}
