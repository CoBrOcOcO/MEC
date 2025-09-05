using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddInstallationConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mec_workflow_ProjectId",
                table: "mec_workflow");

            migrationBuilder.RenameIndex(
                name: "IX_mec_project_ProjectNumber",
                table: "mec_project",
                newName: "IX_Project_ProjectNumber");

            migrationBuilder.RenameIndex(
                name: "IX_mec_installation_pdf_WorkflowId_PdfType",
                table: "mec_installation_pdf",
                newName: "IX_InstallationPdf_WorkflowId_PdfType");

            migrationBuilder.CreateTable(
                name: "mec_installation_configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MacAddress = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SubnetMask = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Gateway = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ComputerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NetworkNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsNetworkConfigured = table.Column<bool>(type: "bit", nullable: false),
                    ConfigurationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfiguredBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChange = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_installation_configuration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_installation_configuration_mec_workflow_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "mec_workflow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workflow_ProjectId_Status",
                table: "mec_workflow",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_mec_installation_configuration_WorkflowId",
                table: "mec_installation_configuration",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mec_installation_configuration");

            migrationBuilder.DropIndex(
                name: "IX_Workflow_ProjectId_Status",
                table: "mec_workflow");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectNumber",
                table: "mec_project",
                newName: "IX_mec_project_ProjectNumber");

            migrationBuilder.RenameIndex(
                name: "IX_InstallationPdf_WorkflowId_PdfType",
                table: "mec_installation_pdf",
                newName: "IX_mec_installation_pdf_WorkflowId_PdfType");

            migrationBuilder.CreateIndex(
                name: "IX_mec_workflow_ProjectId",
                table: "mec_workflow",
                column: "ProjectId");
        }
    }
}
