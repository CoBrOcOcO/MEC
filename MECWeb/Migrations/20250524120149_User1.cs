using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class User1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "mec_user",
                newName: "GivenName");

            migrationBuilder.RenameColumn(
                name: "AccountName",
                table: "mec_user",
                newName: "DisplayName");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "mec_user",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPhoto",
                table: "mec_user",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "mec_user");

            migrationBuilder.DropColumn(
                name: "UserPhoto",
                table: "mec_user");

            migrationBuilder.RenameColumn(
                name: "GivenName",
                table: "mec_user",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "mec_user",
                newName: "AccountName");
        }
    }
}
