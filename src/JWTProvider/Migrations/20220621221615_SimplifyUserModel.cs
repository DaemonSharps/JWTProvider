using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JWTProvider.Migrations
{
    public partial class SimplifyUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRoles_RoleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Passwords",
                keyColumn: "UserId",
                keyValue: new Guid("851dd5e1-28fb-4e1f-8079-07e62c785b09"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("851dd5e1-28fb-4e1f-8079-07e62c785b09"));

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "MiddleName" },
                values: new object[] { new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"), "test@mail.ru", "Денис", "Алексеевич", "Смирнов" });

            migrationBuilder.InsertData(
                table: "Passwords",
                columns: new[] { "UserId", "Hash" },
                values: new object[] { new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"), "mRytDVsoZEPR+eMiMbl/xMAckvL5s+k70iboHYpSIlw=" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Passwords",
                keyColumn: "UserId",
                keyValue: new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"));

            migrationBuilder.AddColumn<long>(
                name: "RoleId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayLogin = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Logins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_UserRoles", x => x.Id));

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1L, "Admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2L, "User" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3L, "App" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "MiddleName", "RoleId" },
                values: new object[] { new Guid("851dd5e1-28fb-4e1f-8079-07e62c785b09"), "test@mail.ru", "Денис", "Алексеевич", "Смирнов", 1L });

            migrationBuilder.InsertData(
                table: "Logins",
                columns: new[] { "UserId", "DisplayLogin" },
                values: new object[] { new Guid("851dd5e1-28fb-4e1f-8079-07e62c785b09"), "Test" });

            migrationBuilder.InsertData(
                table: "Passwords",
                columns: new[] { "UserId", "Hash" },
                values: new object[] { new Guid("851dd5e1-28fb-4e1f-8079-07e62c785b09"), "I3UX9g/lL94qcF4CNNtRiGnhP0E=" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRoles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "UserRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
