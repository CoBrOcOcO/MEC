using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class CreateHardwareFieldTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mec_hardware_field",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HardwareComputerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidationRules = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChange = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_hardware_field", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_hardware_field_mec_hardware_computer_HardwareComputerId",
                        column: x => x.HardwareComputerId,
                        principalTable: "mec_hardware_computer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mec_hardware_field_HardwareComputerId",
                table: "mec_hardware_field",
                column: "HardwareComputerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mec_hardware_field");
        }
    }
}
