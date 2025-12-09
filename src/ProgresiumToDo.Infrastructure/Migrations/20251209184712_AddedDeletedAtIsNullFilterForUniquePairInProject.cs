using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeletedAtIsNullFilterForUniquePairInProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_projects_user_id_name",
                table: "projects");

            migrationBuilder.CreateIndex(
                name: "ix_projects_user_id_name",
                table: "projects",
                columns: new[] { "user_id", "name" },
                unique: true,
                filter: "\"deleted_at\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_projects_user_id_name",
                table: "projects");

            migrationBuilder.CreateIndex(
                name: "ix_projects_user_id_name",
                table: "projects",
                columns: new[] { "user_id", "name" },
                unique: true);
        }
    }
}
