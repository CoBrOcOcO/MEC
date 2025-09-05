using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddInstallationPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mec_installation_pdf",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PdfType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_installation_pdf", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_installation_pdf_mec_workflow_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "mec_workflow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mec_installation_pdf_WorkflowId_PdfType",
                table: "mec_installation_pdf",
                columns: new[] { "WorkflowId", "PdfType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mec_installation_pdf");
        }
    }
}
