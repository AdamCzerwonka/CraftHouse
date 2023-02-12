using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftHouse.Web.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOptionValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OptionValues",
                table: "OptionValues");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "OptionValues",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OptionValues",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "Required",
                table: "Options",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OptionValues",
                table: "OptionValues",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OptionValues_OptionId",
                table: "OptionValues",
                column: "OptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OptionValues",
                table: "OptionValues");

            migrationBuilder.DropIndex(
                name: "IX_OptionValues_OptionId",
                table: "OptionValues");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OptionValues");

            migrationBuilder.DropColumn(
                name: "Required",
                table: "Options");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "OptionValues",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OptionValues",
                table: "OptionValues",
                columns: new[] { "OptionId", "Value" });
        }
    }
}
