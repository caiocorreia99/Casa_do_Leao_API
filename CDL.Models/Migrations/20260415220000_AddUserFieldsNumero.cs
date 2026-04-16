using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDL.Models.Migrations
{
    /// <inheritdoc />
    [Migration("20260415220000_AddUserFieldsNumero")]
    public class AddUserFieldsNumero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "UserFields",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Numero",
                table: "UserFields");
        }
    }
}
