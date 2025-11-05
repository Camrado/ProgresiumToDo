using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserAndApplicationUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "application_user_id",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_users_application_user_id",
                table: "users",
                column: "application_user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_users_asp_net_users_application_user_id",
                table: "users",
                column: "application_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_asp_net_users_application_user_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_application_user_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "application_user_id",
                table: "users");
        }
    }
}
