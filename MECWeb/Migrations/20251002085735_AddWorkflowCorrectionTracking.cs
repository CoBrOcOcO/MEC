using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowCorrectionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectionNote",
                table: "mec_workflow",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectionPhase",
                table: "mec_workflow",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CorrectionRequestedAt",
                table: "mec_workflow",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectionRequestedBy",
                table: "mec_workflow",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPendingCorrection",
                table: "mec_workflow",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectionNote",
                table: "mec_workflow");

            migrationBuilder.DropColumn(
                name: "CorrectionPhase",
                table: "mec_workflow");

            migrationBuilder.DropColumn(
                name: "CorrectionRequestedAt",
                table: "mec_workflow");

            migrationBuilder.DropColumn(
                name: "CorrectionRequestedBy",
                table: "mec_workflow");

            migrationBuilder.DropColumn(
                name: "HasPendingCorrection",
                table: "mec_workflow");
        }
    }
}
