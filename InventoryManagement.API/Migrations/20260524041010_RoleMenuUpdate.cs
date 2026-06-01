using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RoleMenuUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "RoleMenus",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_MenuId",
                table: "RoleMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_UserId",
                table: "RoleMenus",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenus_Menus_MenuId",
                table: "RoleMenus",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenus_Roles_RoleId",
                table: "RoleMenus",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenus_Users_UserId",
                table: "RoleMenus",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenus_Menus_MenuId",
                table: "RoleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenus_Roles_RoleId",
                table: "RoleMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenus_Users_UserId",
                table: "RoleMenus");

            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_MenuId",
                table: "RoleMenus");

            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_UserId",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RoleMenus");
        }
    }
}
