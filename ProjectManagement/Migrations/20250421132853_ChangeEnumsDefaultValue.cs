using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEnumsDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("3485751c-4840-41fa-bfce-8054d8756cee"),
                column: "Role",
                value: 1);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("96a51fca-20ab-4bfc-be2f-b37e79632d06"),
                column: "Role",
                value: 4);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("c6551a2c-e187-49c0-b929-4bba525b14ae"),
                column: "Role",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("3485751c-4840-41fa-bfce-8054d8756cee"),
                column: "Role",
                value: 0);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("96a51fca-20ab-4bfc-be2f-b37e79632d06"),
                column: "Role",
                value: 3);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("c6551a2c-e187-49c0-b929-4bba525b14ae"),
                column: "Role",
                value: 2);
        }
    }
}
