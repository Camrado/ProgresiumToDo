using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgresiumToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmailVerificationFieldsToAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email_verification_code",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "email_verification_code_expires_on",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email_verification_code",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "email_verification_code_expires_on",
                table: "AspNetUsers");
        }
    }
}
