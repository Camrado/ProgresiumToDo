using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationshipsForOnDeleteOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tags_projects_project_id",
                table: "tags");

            migrationBuilder.DropForeignKey(
                name: "fk_task_items_projects_project_id",
                table: "task_items");

            migrationBuilder.DropForeignKey(
                name: "fk_task_items_task_items_parent_task_item_id",
                table: "task_items");

            migrationBuilder.DropForeignKey(
                name: "fk_task_orders_projects_project_id",
                table: "task_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_task_orders_task_items_parent_task_id",
                table: "task_orders");

            migrationBuilder.AddForeignKey(
                name: "fk_tags_projects_project_id",
                table: "tags",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_task_items_projects_project_id",
                table: "task_items",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_task_items_task_items_parent_task_item_id",
                table: "task_items",
                column: "parent_task_item_id",
                principalTable: "task_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_task_orders_projects_project_id",
                table: "task_orders",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_task_orders_task_items_parent_task_id",
                table: "task_orders",
                column: "parent_task_id",
                principalTable: "task_items",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tags_projects_project_id",
                table: "tags");

            migrationBuilder.DropForeignKey(
                name: "fk_task_items_projects_project_id",
                table: "task_items");

            migrationBuilder.DropForeignKey(
                name: "fk_task_items_task_items_parent_task_item_id",
                table: "task_items");

            migrationBuilder.DropForeignKey(
                name: "fk_task_orders_projects_project_id",
                table: "task_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_task_orders_task_items_parent_task_id",
                table: "task_orders");

            migrationBuilder.AddForeignKey(
                name: "fk_tags_projects_project_id",
                table: "tags",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_task_items_projects_project_id",
                table: "task_items",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_task_items_task_items_parent_task_item_id",
                table: "task_items",
                column: "parent_task_item_id",
                principalTable: "task_items",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_task_orders_projects_project_id",
                table: "task_orders",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_task_orders_task_items_parent_task_id",
                table: "task_orders",
                column: "parent_task_id",
                principalTable: "task_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
