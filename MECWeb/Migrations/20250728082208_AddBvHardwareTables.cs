using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddBvHardwareTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mec_bv_hardware_computer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PcType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PcTypeDetails = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChange = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_bv_hardware_computer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_bv_hardware_computer_mec_workflow_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "mec_workflow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mec_bv_hardware_component",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BvHardwareComputerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ComponentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_bv_hardware_component", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_bv_hardware_component_mec_bv_hardware_computer_BvHardwareComputerId",
                        column: x => x.BvHardwareComputerId,
                        principalTable: "mec_bv_hardware_computer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mec_bv_hardware_component_BvHardwareComputerId",
                table: "mec_bv_hardware_component",
                column: "BvHardwareComputerId");

            migrationBuilder.CreateIndex(
                name: "IX_mec_bv_hardware_computer_WorkflowId",
                table: "mec_bv_hardware_computer",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mec_bv_hardware_component");

            migrationBuilder.DropTable(
                name: "mec_bv_hardware_computer");
        }
    }
}
