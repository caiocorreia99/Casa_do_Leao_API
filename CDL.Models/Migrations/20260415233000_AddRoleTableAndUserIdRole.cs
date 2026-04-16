using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDL.Models.Migrations
{
    /// <inheritdoc />
    [Migration("20260415233000_AddRoleTableAndUserIdRole")]
    public class AddRoleTableAndUserIdRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    IdRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.IdRole);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Name", "Code" },
                values: new object[,]
                {
                    { "Usuario", "simple" },
                    { "Editor", "editor" },
                    { "Administrador", "admin" },
                });

            migrationBuilder.AddColumn<int>(
                name: "IdRole",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE `User` u
SET u.`IdRole` = (
  CASE
    WHEN LOWER(TRIM(IFNULL(u.`Role`, ''))) = 'admin' THEN (SELECT r.`IdRole` FROM `Role` r WHERE r.`Code` = 'admin' LIMIT 1)
    WHEN LOWER(TRIM(IFNULL(u.`Role`, ''))) = 'editor' THEN (SELECT r.`IdRole` FROM `Role` r WHERE r.`Code` = 'editor' LIMIT 1)
    WHEN u.`Admin` = 1 THEN (SELECT r.`IdRole` FROM `Role` r WHERE r.`Code` = 'admin' LIMIT 1)
    ELSE (SELECT r.`IdRole` FROM `Role` r WHERE r.`Code` = 'simple' LIMIT 1)
  END
);");

            migrationBuilder.Sql(
                "UPDATE `User` SET `IdRole` = (SELECT `IdRole` FROM `Role` WHERE `Code` = 'simple' LIMIT 1) WHERE `IdRole` IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "IdRole",
                table: "User",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_IdRole",
                table: "User",
                column: "IdRole");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_IdRole",
                table: "User",
                column: "IdRole",
                principalTable: "Role",
                principalColumn: "IdRole",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "Role",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_IdRole",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IdRole",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "User",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"
UPDATE `User` u
INNER JOIN `Role` r ON u.`IdRole` = r.`IdRole`
SET u.`Role` = r.`Code`, u.`Admin` = (r.`Code` = 'admin');");

            migrationBuilder.DropColumn(
                name: "IdRole",
                table: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
