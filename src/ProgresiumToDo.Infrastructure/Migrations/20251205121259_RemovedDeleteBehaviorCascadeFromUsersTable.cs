using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDeleteBehaviorCascadeFromUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_asp_net_users_application_user_id",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_users_asp_net_users_application_user_id",
                table: "users",
                column: "application_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_asp_net_users_application_user_id",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_users_asp_net_users_application_user_id",
                table: "users",
                column: "application_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
