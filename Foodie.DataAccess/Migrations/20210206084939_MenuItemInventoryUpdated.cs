using Microsoft.EntityFrameworkCore.Migrations;

namespace Foodie.DataAccess.Migrations
{
    public partial class MenuItemInventoryUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemInventories_UnitTypes_UnitTypeId",
                table: "MenuItemInventories");

            migrationBuilder.DropIndex(
                name: "IX_MenuItemInventories_UnitTypeId",
                table: "MenuItemInventories");

            migrationBuilder.DropColumn(
                name: "UnitTypeId",
                table: "MenuItemInventories");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "MenuItemInventories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemInventories_InventoryId",
                table: "MenuItemInventories",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemInventories_Inventories_InventoryId",
                table: "MenuItemInventories",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemInventories_Inventories_InventoryId",
                table: "MenuItemInventories");

            migrationBuilder.DropIndex(
                name: "IX_MenuItemInventories_InventoryId",
                table: "MenuItemInventories");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "MenuItemInventories");

            migrationBuilder.AddColumn<int>(
                name: "UnitTypeId",
                table: "MenuItemInventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemInventories_UnitTypeId",
                table: "MenuItemInventories",
                column: "UnitTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemInventories_UnitTypes_UnitTypeId",
                table: "MenuItemInventories",
                column: "UnitTypeId",
                principalTable: "UnitTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
