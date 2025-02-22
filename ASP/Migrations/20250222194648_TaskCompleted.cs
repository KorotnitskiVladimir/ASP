using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.Migrations
{
    /// <inheritdoc />
    public partial class TaskCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                schema: "ASP",
                table: "UsersData",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<float>(
                name: "FootSize",
                schema: "ASP",
                table: "UsersData",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TorsoSize",
                schema: "ASP",
                table: "UsersData",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FootSize",
                schema: "ASP",
                table: "UsersData");

            migrationBuilder.DropColumn(
                name: "TorsoSize",
                schema: "ASP",
                table: "UsersData");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                schema: "ASP",
                table: "UsersData",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
