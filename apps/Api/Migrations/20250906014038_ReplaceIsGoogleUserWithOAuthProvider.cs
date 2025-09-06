using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIsGoogleUserWithOAuthProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGoogleUser",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "OAuthProvider",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OAuthProvider",
                table: "AspNetUsers",
                column: "OAuthProvider");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OAuthProvider",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OAuthProvider",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleUser",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
