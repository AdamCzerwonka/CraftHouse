using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftHouse.Web.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderItemOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderItemOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "OrderItemOptions",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderItemOptions");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "OrderItemOptions");
        }
    }
}
