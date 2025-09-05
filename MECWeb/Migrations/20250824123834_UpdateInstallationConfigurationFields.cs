using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInstallationConfigurationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mec_installation_configuration_WorkflowId",
                table: "mec_installation_configuration");

            migrationBuilder.RenameColumn(
                name: "ConfigurationDate",
                table: "mec_installation_configuration",
                newName: "NetworkConfiguredDate");

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

            migrationBuilder.AddColumn<string>(
                name: "AdditionalPorts",
                table: "mec_installation_configuration",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
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

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "mec_installation_configuration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NetworkAdapterName",
                table: "mec_installation_configuration",
                type: "nvarchar(200)",
                maxLength: 200,
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

            migrationBuilder.CreateIndex(
                name: "IX_InstallationConfiguration_WorkflowId",
                table: "mec_installation_configuration",
                column: "WorkflowId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstallationConfiguration_WorkflowId",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "AdditionalPorts",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
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
                name: "IsActive",
                table: "mec_installation_configuration");

            migrationBuilder.DropColumn(
                name: "NetworkAdapterName",
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

            migrationBuilder.RenameColumn(
                name: "NetworkConfiguredDate",
                table: "mec_installation_configuration",
                newName: "ConfigurationDate");

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

            migrationBuilder.CreateIndex(
                name: "IX_mec_installation_configuration_WorkflowId",
                table: "mec_installation_configuration",
                column: "WorkflowId");
        }
    }
}
