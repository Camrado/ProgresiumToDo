using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUniqueIndexButLeftIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_verification_codes_application_user_id_type",
                table: "verification_codes");

            migrationBuilder.CreateIndex(
                name: "ix_verification_codes_application_user_id_type",
                table: "verification_codes",
                columns: new[] { "application_user_id", "type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_verification_codes_application_user_id_type",
                table: "verification_codes");

            migrationBuilder.CreateIndex(
                name: "ix_verification_codes_application_user_id_type",
                table: "verification_codes",
                columns: new[] { "application_user_id", "type" },
                unique: true);
        }
    }
}
