using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedProjectIdAndColorFromTagEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tags_projects_project_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_tags_project_id_name",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "color",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "project_id",
                table: "tags");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_name",
                table: "tags");

            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "project_id",
                table: "tags",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_tags_project_id_name",
                table: "tags",
                columns: new[] { "project_id", "name" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");

            migrationBuilder.AddForeignKey(
                name: "fk_tags_projects_project_id",
                table: "tags",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
