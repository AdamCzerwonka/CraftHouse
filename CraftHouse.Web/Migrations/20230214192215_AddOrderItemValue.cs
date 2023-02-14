using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftHouse.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "OrderItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "OrderItems");
        }
    }
}
