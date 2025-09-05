using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MECWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mec_project",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChange = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mec_translate_dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastChange = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_translate_dictionary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mec_user",
                columns: table => new
                {
                    UId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SurName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EMail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_user", x => x.UId);
                });

            migrationBuilder.CreateTable(
                name: "mec_machine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_machine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_machine_mec_project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "mec_project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mec_translate_project",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TranslationFileType = table.Column<int>(type: "int", nullable: false),
                    TranslationService = table.Column<int>(type: "int", nullable: false),
                    TranslatedItemsCount = table.Column<int>(type: "int", nullable: false),
                    TotalItemsCount = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_translate_project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_translate_project_mec_project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "mec_project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mec_translate_entry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DictionaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_translate_entry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_translate_entry_mec_translate_dictionary_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "mec_translate_dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mec_user_project",
                columns: table => new
                {
                    DbUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DbProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_user_project", x => new { x.DbUserId, x.DbProjectId });
                    table.ForeignKey(
                        name: "FK_mec_user_project_mec_project_DbProjectId",
                        column: x => x.DbProjectId,
                        principalTable: "mec_project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mec_user_project_mec_user_DbUserId",
                        column: x => x.DbUserId,
                        principalTable: "mec_user",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mec_user_project_favorite",
                columns: table => new
                {
                    DbUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DbProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_user_project_favorite", x => new { x.DbUserId, x.DbProjectId });
                    table.ForeignKey(
                        name: "FK_mec_user_project_favorite_mec_project_DbProjectId",
                        column: x => x.DbProjectId,
                        principalTable: "mec_project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mec_user_project_favorite_mec_user_DbUserId",
                        column: x => x.DbUserId,
                        principalTable: "mec_user",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mec_translate_translation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mec_translate_translation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mec_translate_translation_mec_translate_entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "mec_translate_entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mec_machine_ProjectId",
                table: "mec_machine",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_mec_translate_entry_DictionaryId",
                table: "mec_translate_entry",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_mec_translate_project_ProjectId",
                table: "mec_translate_project",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_mec_translate_translation_EntryId",
                table: "mec_translate_translation",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_mec_user_project_DbProjectId",
                table: "mec_user_project",
                column: "DbProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_mec_user_project_favorite_DbProjectId",
                table: "mec_user_project_favorite",
                column: "DbProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mec_machine");

            migrationBuilder.DropTable(
                name: "mec_translate_project");

            migrationBuilder.DropTable(
                name: "mec_translate_translation");

            migrationBuilder.DropTable(
                name: "mec_user_project");

            migrationBuilder.DropTable(
                name: "mec_user_project_favorite");

            migrationBuilder.DropTable(
                name: "mec_translate_entry");

            migrationBuilder.DropTable(
                name: "mec_project");

            migrationBuilder.DropTable(
                name: "mec_user");

            migrationBuilder.DropTable(
                name: "mec_translate_dictionary");
        }
    }
}
