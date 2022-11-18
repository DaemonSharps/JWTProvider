using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JWTProvider.Migrations
{
    public partial class UserLoginPasswordRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_UserRoles", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Passwords",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Passwords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Passwords");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
