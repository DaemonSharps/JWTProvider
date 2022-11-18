using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTProvider.Migrations
{
    public partial class version2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MiddleName",
                table: "Users",
                newName: "Patronymic");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FinishDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "AppTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatingSystemTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingSystemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Apps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    AppTypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apps_AppTypes_AppTypeId",
                        column: x => x.AppTypeId,
                        principalTable: "AppTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshToken = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppId = table.Column<Guid>(type: "uuid", nullable: true),
                    IP = table.Column<string>(type: "text", nullable: true),
                    OperatingSystemTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FinishDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "Apps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sessions_OperatingSystemTypes_OperatingSystemTypeId",
                        column: x => x.OperatingSystemTypeId,
                        principalTable: "OperatingSystemTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppTypes",
                columns: new[] { "Id", "Code" },
                values: new object[] { new Guid("4c175cf1-67a7-4b35-a978-9b79acf39743"), "Browser" });

            migrationBuilder.InsertData(
                table: "OperatingSystemTypes",
                columns: new[] { "Id", "Code" },
                values: new object[] { new Guid("7486becb-b36c-4e79-9b1a-a0e49240ae3c"), "Windows" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"),
                columns: new[] { "LastName", "Patronymic" },
                values: new object[] { "Смирнов", "Алексеевич" });

            migrationBuilder.InsertData(
                table: "Apps",
                columns: new[] { "Id", "AppTypeId", "Code" },
                values: new object[] { new Guid("6544598e-f174-41dd-a938-a0ecc5244c4d"), new Guid("4c175cf1-67a7-4b35-a978-9b79acf39743"), "Yandex" });

            migrationBuilder.CreateIndex(
                name: "IX_Apps_AppTypeId",
                table: "Apps",
                column: "AppTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AppId",
                table: "Sessions",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_OperatingSystemTypeId",
                table: "Sessions",
                column: "OperatingSystemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Apps");

            migrationBuilder.DropTable(
                name: "OperatingSystemTypes");

            migrationBuilder.DropTable(
                name: "AppTypes");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FinishDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Patronymic",
                table: "Users",
                newName: "MiddleName");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"),
                columns: new[] { "LastName", "MiddleName" },
                values: new object[] { "Алексеевич", "Смирнов" });
        }
    }
}
