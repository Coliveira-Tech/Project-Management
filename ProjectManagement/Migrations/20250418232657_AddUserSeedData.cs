using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Name", "Password", "Role" },
                values: new object[,]
                {
                    { new Guid("3485751c-4840-41fa-bfce-8054d8756cee"), "bill@microsoft.com", "Bill Gates", "123456", 0 },
                    { new Guid("96a51fca-20ab-4bfc-be2f-b37e79632d06"), "linus@linux.org", "Linus Torvalds", "#ViTeam#35", 3 },
                    { new Guid("c6551a2c-e187-49c0-b929-4bba525b14ae"), "steve@apple.com", "Stive Jobs", "@MacAttack22!", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("3485751c-4840-41fa-bfce-8054d8756cee"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("96a51fca-20ab-4bfc-be2f-b37e79632d06"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("c6551a2c-e187-49c0-b929-4bba525b14ae"));
        }
    }
}
