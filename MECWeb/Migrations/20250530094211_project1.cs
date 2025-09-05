using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class project1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "mec_project");

            migrationBuilder.CreateIndex(
                name: "IX_mec_project_ProjectNumber",
                table: "mec_project",
                column: "ProjectNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mec_project_ProjectNumber",
                table: "mec_project");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "mec_project",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
