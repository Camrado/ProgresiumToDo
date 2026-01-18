using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTablesRelatedToFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "absolute_limit",
                table: "plan_features",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "absolute_limit",
                table: "plan_features");
        }
    }
}
