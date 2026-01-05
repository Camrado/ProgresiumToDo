using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueConstraintToTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_project_id",
                table: "tags");

            migrationBuilder.CreateIndex(
                name: "ix_tags_project_id_name",
                table: "tags",
                columns: new[] { "project_id", "name" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_project_id_name",
                table: "tags");

            migrationBuilder.CreateIndex(
                name: "ix_tags_project_id",
                table: "tags",
                column: "project_id");
        }
    }
}
