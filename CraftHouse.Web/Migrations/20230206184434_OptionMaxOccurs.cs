using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftHouse.Web.Migrations
{
    /// <inheritdoc />
    public partial class OptionMaxOccurs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxOccurs",
                table: "Options",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxOccurs",
                table: "Options");
        }
    }
}
