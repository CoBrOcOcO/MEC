using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNetworkFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mec_installation_pdf");

            migrationBuilder.DropColumn(
                name: "AdditionalPorts",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "ConfiguredBy",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "DnsServer1",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "DnsServer2",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "EnableFirewall",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "EnableRemoteDesktop",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "EnableVpnAccess",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "FirewallRules",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "Gateway",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "NetworkAdapterName",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "NetworkConfiguredDate",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "SubnetMask",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "UseDhcp",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "VlanId",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "VpnConfiguration",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "Workgroup",
                table: "mec_installation_configuration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalPorts",
                table: "mec_installation_configuration",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfiguredBy",
                table: "mec_installation_configuration",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnsServer1",
                table: "mec_installation_configuration",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DnsServer2",
                table: "mec_installation_configuration",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "mec_installation_configuration",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnableFirewall",
                table: "mec_installation_configuration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableRemoteDesktop",
                table: "mec_installation_configuration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableVpnAccess",
                table: "mec_installation_configuration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FirewallRules",
                table: "mec_installation_configuration",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gateway",
                table: "mec_installation_configuration",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetworkAdapterName",
                table: "mec_installation_configuration",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NetworkConfiguredDate",
                table: "mec_installation_configuration",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubnetMask",
                table: "mec_installation_configuration",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseDhcp",
                table: "mec_installation_configuration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VlanId",
                table: "mec_installation_configuration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VpnConfiguration",
                table: "mec_installation_configuration",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Workgroup",
                table: "mec_installation_configuration",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

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
                name: "IX_InstallationPdf_WorkflowId_PdfType",
                table: "mec_installation_pdf",
                columns: new[] { "WorkflowId", "PdfType" });
        }
    }
}
