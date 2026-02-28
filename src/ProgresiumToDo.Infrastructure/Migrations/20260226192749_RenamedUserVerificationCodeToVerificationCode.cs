using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedUserVerificationCodeToVerificationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_verification_codes_asp_net_users_application_user_id",
                table: "user_verification_codes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_verification_codes",
                table: "user_verification_codes");

            migrationBuilder.RenameTable(
                name: "user_verification_codes",
                newName: "verification_codes");

            migrationBuilder.RenameIndex(
                name: "ix_user_verification_codes_application_user_id",
                table: "verification_codes",
                newName: "ix_verification_codes_application_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_verification_codes",
                table: "verification_codes",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_verification_codes_asp_net_users_application_user_id",
                table: "verification_codes",
                column: "application_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_verification_codes_asp_net_users_application_user_id",
                table: "verification_codes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_verification_codes",
                table: "verification_codes");

            migrationBuilder.RenameTable(
                name: "verification_codes",
                newName: "user_verification_codes");

            migrationBuilder.RenameIndex(
                name: "ix_verification_codes_application_user_id",
                table: "user_verification_codes",
                newName: "ix_user_verification_codes_application_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_verification_codes",
                table: "user_verification_codes",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_verification_codes_asp_net_users_application_user_id",
                table: "user_verification_codes",
                column: "application_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
