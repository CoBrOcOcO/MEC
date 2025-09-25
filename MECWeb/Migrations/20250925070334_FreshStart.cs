using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class FreshStart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mec_machine_mec_project_ProjectId",
                table: "mec_machine");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_translate_project_mec_project_ProjectId",
                table: "mec_translate_project");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_user_project_mec_project_DbProjectId",
                table: "mec_user_project");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_user_project_favorite_mec_project_DbProjectId",
                table: "mec_user_project_favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_workflow_mec_project_ProjectId",
                table: "mec_workflow");

            migrationBuilder.DropTable(
                name: "mec_installation_pdf");

            migrationBuilder.DropIndex(
                name: "IX_mec_installation_configuration_WorkflowId",
                table: "mec_installation_configuration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mec_project",
                table: "mec_project");

            migrationBuilder.DropColumn(
                name: "ConfigurationDate",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "Gateway",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "SubnetMask",
                table: "mec_installation_configuration");

            migrationBuilder.RenameTable(
                name: "mec_project",
                newName: "Project");

            migrationBuilder.RenameColumn(
                name: "ConfiguredBy",
                table: "mec_installation_configuration",
                newName: "CreatedBy");

            migrationBuilder.AlterColumn<string>(
                name: "ComputerName",
                table: "mec_installation_configuration",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "mec_installation_configuration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectNumber",
                table: "Project",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Project",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Project",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GitCreatedAt",
                table: "Project",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitDescription",
                table: "Project",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GitEnabled",
                table: "Project",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GitIsPrivate",
                table: "Project",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GitOwner",
                table: "Project",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitRepositoryName",
                table: "Project",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitRepositoryUrl",
                table: "Project",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationConfiguration_WorkflowId",
                table: "mec_installation_configuration",
                column: "WorkflowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_machine_Project_ProjectId",
                table: "mec_machine",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_translate_project_Project_ProjectId",
                table: "mec_translate_project",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_user_project_Project_DbProjectId",
                table: "mec_user_project",
                column: "DbProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_user_project_favorite_Project_DbProjectId",
                table: "mec_user_project_favorite",
                column: "DbProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_workflow_Project_ProjectId",
                table: "mec_workflow",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mec_machine_Project_ProjectId",
                table: "mec_machine");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_translate_project_Project_ProjectId",
                table: "mec_translate_project");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_user_project_Project_DbProjectId",
                table: "mec_user_project");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_user_project_favorite_Project_DbProjectId",
                table: "mec_user_project_favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_workflow_Project_ProjectId",
                table: "mec_workflow");

            migrationBuilder.DropIndex(
                name: "IX_InstallationConfiguration_WorkflowId",
                table: "mec_installation_configuration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "GitCreatedAt",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "GitDescription",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "GitEnabled",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "GitIsPrivate",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "GitOwner",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "GitRepositoryName",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "GitRepositoryUrl",
                table: "Project");

            migrationBuilder.RenameTable(
                name: "Project",
                newName: "mec_project");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "mec_installation_configuration",
                newName: "ConfiguredBy");

            migrationBuilder.AlterColumn<string>(
                name: "ComputerName",
                table: "mec_installation_configuration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfigurationDate",
                table: "mec_installation_configuration",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gateway",
                table: "mec_installation_configuration",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubnetMask",
                table: "mec_installation_configuration",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectNumber",
                table: "mec_project",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "mec_project",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "mec_project",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_mec_project",
                table: "mec_project",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "mec_installation_pdf",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PdfType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
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
                name: "IX_mec_installation_configuration_WorkflowId",
                table: "mec_installation_configuration",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationPdf_WorkflowId_PdfType",
                table: "mec_installation_pdf",
                columns: new[] { "WorkflowId", "PdfType" });

            migrationBuilder.AddForeignKey(
                name: "FK_mec_machine_mec_project_ProjectId",
                table: "mec_machine",
                column: "ProjectId",
                principalTable: "mec_project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_translate_project_mec_project_ProjectId",
                table: "mec_translate_project",
                column: "ProjectId",
                principalTable: "mec_project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_user_project_mec_project_DbProjectId",
                table: "mec_user_project",
                column: "DbProjectId",
                principalTable: "mec_project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_user_project_favorite_mec_project_DbProjectId",
                table: "mec_user_project_favorite",
                column: "DbProjectId",
                principalTable: "mec_project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mec_workflow_mec_project_ProjectId",
                table: "mec_workflow",
                column: "ProjectId",
                principalTable: "mec_project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
