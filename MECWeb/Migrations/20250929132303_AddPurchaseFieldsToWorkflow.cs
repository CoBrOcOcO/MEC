using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseFieldsToWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PurchaseComment",
                table: "mec_workflow",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PurchaseCompleted",
                table: "mec_workflow",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseCompletedDate",
                table: "mec_workflow",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseComment",
                table: "mec_workflow");

            migrationBuilder.DropColumn(
                name: "PurchaseCompleted",
                table: "mec_workflow");

            migrationBuilder.DropColumn(
                name: "PurchaseCompletedDate",
                table: "mec_workflow");
        }
    }
}
