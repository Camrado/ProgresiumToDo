using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedVerificationCodeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_verification_codes_application_user_id",
                table: "verification_codes");

            migrationBuilder.AlterColumn<string>(
                name: "code_hash",
                table: "verification_codes",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_verification_codes_application_user_id_type",
                table: "verification_codes",
                columns: new[] { "application_user_id", "type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_verification_codes_application_user_id_type",
                table: "verification_codes");

            migrationBuilder.AlterColumn<string>(
                name: "code_hash",
                table: "verification_codes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "ix_verification_codes_application_user_id",
                table: "verification_codes",
                column: "application_user_id");
        }
    }
}
