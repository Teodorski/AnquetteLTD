using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnquetteLTD.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusAndWindowToAnquette : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndsAt",
                table: "Anquettes",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Anquettes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartsAt",
                table: "Anquettes",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndsAt",
                table: "Anquettes");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Anquettes");

            migrationBuilder.DropColumn(
                name: "StartsAt",
                table: "Anquettes");
        }
    }
}
