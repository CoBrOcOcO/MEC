using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class FixBvHardwareCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mec_bv_hardware_computer_mec_workflow_WorkflowId",
                table: "mec_bv_hardware_computer");

            migrationBuilder.AddColumn<Guid>(
                name: "BvHardwareComputerId",
                table: "mec_configuration_entry",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "mec_bv_hardware_computer",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mec_configuration_entry_BvHardwareComputerId",
                table: "mec_configuration_entry",
                column: "BvHardwareComputerId");

            migrationBuilder.AddForeignKey(
                name: "FK_mec_bv_hardware_computer_mec_workflow_WorkflowId",
                table: "mec_bv_hardware_computer",
                column: "WorkflowId",
                principalTable: "mec_workflow",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_mec_configuration_entry_mec_bv_hardware_computer_BvHardwareComputerId",
                table: "mec_configuration_entry",
                column: "BvHardwareComputerId",
                principalTable: "mec_bv_hardware_computer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mec_bv_hardware_computer_mec_workflow_WorkflowId",
                table: "mec_bv_hardware_computer");

            migrationBuilder.DropForeignKey(
                name: "FK_mec_configuration_entry_mec_bv_hardware_computer_BvHardwareComputerId",
                table: "mec_configuration_entry");

            migrationBuilder.DropIndex(
                name: "IX_mec_configuration_entry_BvHardwareComputerId",
                table: "mec_configuration_entry");

            migrationBuilder.DropColumn(
                name: "BvHardwareComputerId",
                table: "mec_configuration_entry");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "mec_bv_hardware_computer");

            migrationBuilder.AddForeignKey(
                name: "FK_mec_bv_hardware_computer_mec_workflow_WorkflowId",
                table: "mec_bv_hardware_computer",
                column: "WorkflowId",
                principalTable: "mec_workflow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
