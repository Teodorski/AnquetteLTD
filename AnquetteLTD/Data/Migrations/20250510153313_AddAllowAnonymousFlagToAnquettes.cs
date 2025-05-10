using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnquetteLTD.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowAnonymousFlagToAnquettes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowAnonymous",
                table: "Anquettes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAnonymous",
                table: "Anquettes");
        }
    }
}
