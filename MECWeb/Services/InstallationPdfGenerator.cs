using MECWeb.DbModels;
using MECWeb.DbModels.Workflow;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MECWeb.Services
{
    public class InstallationPdfGenerator
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<InstallationPdfGenerator> _logger;
        private readonly string _logoPath;

        public InstallationPdfGenerator(
            ApplicationDbContext dbContext,
            ILogger<InstallationPdfGenerator> logger,
            IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logoPath = Path.Combine(env.WebRootPath, "Assets", "Schaeffler_green_rgb_150px.png");
        }

        /// <summary>
        /// Generate PDF for BDR Installation with modern design
        /// </summary>
        public async Task<byte[]> GenerateBdrInstallationPdfAsync(Guid workflowId)
        {
            var data = await LoadBdrDataAsync(workflowId);

            if (data.Workflow == null)
            {
                throw new InvalidOperationException("Workflow not found");
            }

            var projectNumber = data.Project?.ProjectNumber ?? "";
            var projectName = data.Project?.Name ?? "";

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => CreateModernHeader(c, "BDR-FORMULAR", projectNumber, projectName));

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Spacing(15);
                        column.Item().Element(c => CreateModernProjectSection(c, data));
                        column.Item().Element(c => CreateModernBdrHardwareSection(c, data));

                        // Separate sections for Hardware Details and Additional Hardware
                        var hardwareFields = data.HardwareFields?.Where(f => f.FieldType == "Hardware" && !string.IsNullOrWhiteSpace(f.FieldValue)).ToList();
                        var additionalFields = data.HardwareFields?.Where(f => f.FieldType == "Additional" && !string.IsNullOrWhiteSpace(f.FieldValue)).ToList();

                        if (hardwareFields?.Any() == true)
                        {
                            column.Item().Element(c => CreateModernBdrHardwareDetailsSection(c, hardwareFields));
                        }

                        if (additionalFields?.Any() == true)
                        {
                            column.Item().Element(c => CreateModernBdrAdditionalHardwareSection(c, additionalFields));
                        }

                        // General Description Hardware - according to Additional Hardware
                        if (data.BdrHardware != null && !string.IsNullOrEmpty(data.BdrHardware.Description))
                        {
                            column.Item().Element(c => CreateModernBdrHardwareRemarksSection(c, data.BdrHardware.Description));
                        }

                        column.Item().Element(c => CreateModernSoftwareSection(c, data));

                        // General software description - according to software configuration
                        var softwareRemarks = ParseSoftwareRemarksFromDescription(
                            data.Workflow?.Description ?? "",
                            data.Workflow?.WorkflowType == WorkflowType.BDR);

                        if (!string.IsNullOrEmpty(softwareRemarks))
                        {
                            column.Item().Element(c => CreateModernSoftwareRemarksSection(c, softwareRemarks));
                        }

                        column.Item().Element(c => CreateModernNetworkSection(c, data));

                        // Comments section - always shown (Installation is always displayed)
                        column.Item().Element(c => CreateModernCommentsSection(c, data));
                    });

                    page.Footer().AlignCenter().DefaultTextStyle(t => t.FontSize(9).FontColor(Colors.Grey.Medium)).Text(x =>
                    {
                        x.Span("Seite ");
                        x.CurrentPageNumber();
                        x.Span(" von ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        /// <summary>
        /// Generate PDF for BV Installation with modern design
        /// </summary>
        public async Task<byte[]> GenerateBvInstallationPdfAsync(Guid workflowId)
        {
            var data = await LoadBvDataAsync(workflowId);

            if (data.Workflow == null)
            {
                throw new InvalidOperationException("Workflow not found");
            }

            var projectNumber = data.Project?.ProjectNumber ?? "";
            var projectName = data.Project?.Name ?? "";

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => CreateModernHeader(c, "BV-FORMULAR", projectNumber, projectName));

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Spacing(15);
                        column.Item().Element(c => CreateModernProjectSection(c, data, isBv: true));
                        column.Item().Element(c => CreateModernBvHardwareSection(c, data));

                        // General hardware description - according to hardware configuration/components
                        if (data.BvHardware != null && !string.IsNullOrEmpty(data.BvHardware.Description))
                        {
                            column.Item().Element(c => CreateModernBvHardwareRemarksSection(c, data.BvHardware.Description));
                        }

                        column.Item().Element(c => CreateModernSoftwareSection(c, data));

                        // General software description - according to software configuration
                        var softwareRemarks = ParseSoftwareRemarksFromDescription(
                            data.Workflow?.Description ?? "",
                            data.Workflow?.WorkflowType == WorkflowType.BDR);

                        if (!string.IsNullOrEmpty(softwareRemarks))
                        {
                            column.Item().Element(c => CreateModernSoftwareRemarksSection(c, softwareRemarks));
                        }

                        column.Item().Element(c => CreateModernNetworkSection(c, data));

                        // Comments section - always shown (Installation is always displayed)
                        column.Item().Element(c => CreateModernCommentsSection(c, data));
                    });

                    page.Footer().AlignCenter().DefaultTextStyle(t => t.FontSize(9).FontColor(Colors.Grey.Medium)).Text(x =>
                    {
                        x.Span("Seite ");
                        x.CurrentPageNumber();
                        x.Span(" von ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        /// <summary>
        /// Create modern header with Schaeffler logo and project identification
        /// </summary>
        private void CreateModernHeader(IContainer container, string title, string projectNumber = "", string projectName = "")
        {
            container.Column(column =>
            {
                // First row: Logo and title
                column.Item().Row(row =>
                {
                    if (File.Exists(_logoPath))
                    {
                        row.ConstantItem(150).Image(_logoPath);
                    }
                    else
                    {
                        row.ConstantItem(150).Text("SCHAEFFLER").FontSize(20).Bold().FontColor("#00873C");
                    }

                    row.RelativeItem();
                    row.ConstantItem(200).AlignRight().AlignMiddle().Text(title).FontSize(16).FontColor(Colors.Grey.Darken2);
                });

                // Second row: Project identification (subtle, in grey)
                if (!string.IsNullOrEmpty(projectNumber) || !string.IsNullOrEmpty(projectName))
                {
                    column.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(3)
                        .Row(row =>
                        {
                            row.RelativeItem().Text(text =>
                            {
                                if (!string.IsNullOrEmpty(projectNumber))
                                {
                                    text.Span("Projekt: ").FontSize(8).FontColor(Colors.Grey.Medium).Bold();
                                    text.Span(projectNumber).FontSize(8).FontColor(Colors.Grey.Darken1);
                                }
                                if (!string.IsNullOrEmpty(projectName))
                                {
                                    if (!string.IsNullOrEmpty(projectNumber))
                                        text.Span(" | ").FontSize(8).FontColor(Colors.Grey.Medium);
                                    text.Span(projectName).FontSize(8).FontColor(Colors.Grey.Darken1);
                                }
                            });
                        });
                }
            });
        }

        /// <summary>
        /// Create modern project information section
        /// </summary>
        private void CreateModernProjectSection(IContainer container, InstallationData data, bool isBv = false)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Projekt-Information").FontSize(13).Bold().FontColor("#00873C");

                column.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(140);
                        columns.RelativeColumn();
                    });

                    // BV: Show contact person only if it has a value
                    if (isBv && data.BvHardware != null && !string.IsNullOrEmpty(data.BvHardware.ContactPerson))
                    {
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text("Ansprechpartner:").FontSize(9).Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(data.BvHardware.ContactPerson).FontSize(9);
                    }

                    // Always show project number
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text("Projektnummer:").FontSize(9).Bold();
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text(data.Project?.ProjectNumber ?? "").FontSize(9);

                    // Always show project name
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text("Projektname:").FontSize(9).Bold();
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text(data.Project?.Name ?? "").FontSize(9);

                    // Show location only if it has a value
                    if (!string.IsNullOrEmpty(data.Project?.Location))
                    {
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text("Standort:").FontSize(9).Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(data.Project.Location).FontSize(9);
                    }

                    // Always show creation date
                    if (data.Workflow != null)
                    {
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text("Erstellt am:").FontSize(9).Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(data.Workflow.CreationDate.ToString("dd.MM.yyyy HH:mm")).FontSize(9);
                    }
                });
            });
        }

        /// <summary>
        /// Create modern BDR hardware section
        /// </summary>
        private void CreateModernBdrHardwareSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Hardware-Konfiguration").FontSize(13).Bold().FontColor("#00873C");

                if (data.BdrHardware != null && !string.IsNullOrEmpty(data.BdrHardware.HardwareSpecs))
                {
                    column.Item().PaddingTop(8).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(140);
                            columns.RelativeColumn();
                        });

                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text("Rechner-Typ:").FontSize(9).Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(data.BdrHardware.HardwareSpecs).FontSize(9);
                    });
                }
                else
                {
                    column.Item().PaddingTop(8).Text("Keine Hardware-Konfiguration gefunden.")
                        .Italic().FontColor(Colors.Grey.Medium);
                }
            });
        }

        /// <summary>
        /// Create modern hardware details section for BDR (FieldType = "Hardware")
        /// </summary>
        private void CreateModernBdrHardwareDetailsSection(IContainer container, List<DbHardwareField> hardwareFields)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Hardware-Details").FontSize(13).Bold().FontColor("#00873C");

                column.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                            .Text("Hardware").FontSize(10).Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                            .Text("Wert").FontSize(10).Bold();
                    });

                    foreach (var field in hardwareFields)
                    {
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(field.DisplayName ?? field.FieldName).FontSize(9);
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(field.FieldValue ?? "").FontSize(9);
                    }
                });
            });
        }

        /// <summary>
        /// Create modern additional hardware section for BDR (FieldType = "Additional")
        /// </summary>
        private void CreateModernBdrAdditionalHardwareSection(IContainer container, List<DbHardwareField> additionalFields)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Zusatzhardware").FontSize(13).Bold().FontColor("#00873C");

                column.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                            .Text("Zusatzhardware").FontSize(10).Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                            .Text("Wert").FontSize(10).Bold();
                    });

                    foreach (var field in additionalFields)
                    {
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(field.DisplayName ?? field.FieldName).FontSize(9);
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(field.FieldValue ?? "").FontSize(9);
                    }
                });
            });
        }

        /// <summary>
        /// Create modern BDR hardware remarks section - shown after Zusatzhardware
        /// </summary>
        private void CreateModernBdrHardwareRemarksSection(IContainer container, string remarks)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Allgemeine Bemerkungen Hardware").FontSize(13).Bold().FontColor("#00873C");

                column.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(remarks).FontSize(9);
            });
        }

        /// <summary>
        /// Create modern BV hardware section
        /// </summary>
        private void CreateModernBvHardwareSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Hardware-Konfiguration").FontSize(13).Bold().FontColor("#00873C");

                if (data.BvHardware != null && !string.IsNullOrEmpty(data.BvHardware.PcType))
                {
                    column.Item().PaddingTop(8).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(140);
                            columns.RelativeColumn();
                        });

                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text("Rechner-Typ:").FontSize(9).Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(data.BvHardware.PcType).FontSize(9);
                    });

                    // Only show hardware components if there are any selected
                    if (data.BvHardwareComponents?.Any(c => c.IsSelected) == true)
                    {
                        column.Item().PaddingTop(15).Element(compContainer =>
                        {
                            compContainer.Column(innerCol =>
                            {
                                innerCol.Item().BorderBottom(1).BorderColor("#00873C").PaddingBottom(5)
                                    .Text("Hardware-Komponenten").FontSize(11).Bold().FontColor("#00873C");

                                innerCol.Item().PaddingTop(8).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.ConstantColumn(60);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                                            .Text("Komponente").FontSize(10).Bold();
                                        header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                                            .Text("Anzahl").FontSize(10).Bold();
                                        header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                                            .Text("Typ").FontSize(10).Bold();
                                        header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                                            .Text("").FontSize(10).Bold();
                                    });

                                    foreach (var component in data.BvHardwareComponents.Where(c => c.IsSelected))
                                    {
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                            .Text(component.ComponentName ?? "-").FontSize(9);
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                            .Text(component.Quantity ?? "1").FontSize(9);
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                            .Text(component.ComponentType ?? "-").FontSize(9);
                                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                            .Text(component.Category ?? "-").FontSize(9);
                                    }
                                });
                            });
                        });
                    }
                }
                else
                {
                    column.Item().PaddingTop(8).Text("Keine Hardware-Konfiguration gefunden.")
                        .Italic().FontColor(Colors.Grey.Medium);
                }
            });
        }

        /// <summary>
        /// Create modern BV hardware remarks section
        /// </summary>
        private void CreateModernBvHardwareRemarksSection(IContainer container, string remarks)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Allgemeine Bemerkungen Hardware").FontSize(13).Bold().FontColor("#00873C");

                column.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(remarks).FontSize(9);
            });
        }

        /// <summary>
        /// Create modern software section
        /// </summary>
        private void CreateModernSoftwareSection(IContainer container, InstallationData data)
        {
            container.ShowOnce().Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Software-Konfiguration").FontSize(13).Bold().FontColor("#00873C");

                var softwareList = ParseSoftwareFromDescription(
                    data.Workflow?.Description ?? "",
                    data.Workflow?.WorkflowType == WorkflowType.BDR);

                if (softwareList.Any())
                {
                    column.Item().PaddingTop(8).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                                .Text("Software").FontSize(10).Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                                .Text("Version/Hinweis").FontSize(10).Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                                .Text("Lizenz").FontSize(10).Bold();
                        });

                        foreach (var software in softwareList)
                        {
                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text(software.Name).FontSize(9);
                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text(software.Version ?? "-").FontSize(9);
                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text(software.HasLicense ? "Ja" : "Nein").FontSize(9);
                        }
                    });
                }
                else
                {
                    column.Item().PaddingTop(8).Text("Keine Software konfiguriert.")
                        .Italic().FontColor(Colors.Grey.Medium);
                }
            });
        }

        /// <summary>
        /// Create modern software remarks section
        /// </summary>
        private void CreateModernSoftwareRemarksSection(IContainer container, string remarks)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Allgemeine Bemerkungen Software").FontSize(13).Bold().FontColor("#00873C");

                column.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(remarks).FontSize(9);
            });
        }

        /// <summary>
        /// Create modern network section
        /// </summary>
        private void CreateModernNetworkSection(IContainer container, InstallationData data)
        {
            container.ShowOnce().Column(column =>
            {
                column.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                    .Text("Netzwerk-Konfiguration").FontSize(13).Bold().FontColor("#00873C");

                // Always show MAC and IP address fields (even if InstallationConfig is null)
                column.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(140);
                        columns.RelativeColumn();
                    });

                    // Always show MAC address
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text("MAC-Adresse").FontSize(9).Bold();
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text(data.InstallationConfig?.MacAddress ?? "").FontSize(9);

                    // Always show IP address
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text("IP-Adresse:").FontSize(9).Bold();
                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text(data.InstallationConfig?.IpAddress ?? "").FontSize(9);
                });

                // Only show network notes if they have content
                if (data.InstallationConfig != null && !string.IsNullOrEmpty(data.InstallationConfig.NetworkNotes))
                {
                    column.Item().PaddingTop(8).Element(notesContainer =>
                    {
                        notesContainer.Column(col =>
                        {
                            col.Item().Text("Netzwerk-Hinweise").FontSize(10).Bold().FontColor(Colors.Grey.Darken1);
                            col.Item().PaddingTop(3).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text(data.InstallationConfig.NetworkNotes).FontSize(9);
                        });
                    });
                }
            });
        }

        /// <summary>
        /// Create modern comments section - Purchase first, then Installation
        /// Installation section is ALWAYS shown (even without comment text)
        /// </summary>
        private void CreateModernCommentsSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                var installationComment = ParseCommentFromDescription(
                    data.Workflow?.Description ?? "", "INSTALLATION_COMMENT:");
                var purchaseComment = ParseCommentFromDescription(
                    data.Workflow?.Description ?? "", "PURCHASE_COMMENT:");

                // 1. PURCHASE SECTION - Only show if there is content
                if (!string.IsNullOrEmpty(purchaseComment))
                {
                    column.Item().ShowOnce().Column(innerColumn =>
                    {
                        innerColumn.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                            .Text("Einkauf Kommentare & Hinweise").FontSize(13).Bold().FontColor("#00873C");

                        innerColumn.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(purchaseComment).FontSize(9);
                    });
                }

                // 2. INSTALLATION SECTION - ALWAYS show (even without comment text)
                // Add spacing if purchase comments were shown
                if (!string.IsNullOrEmpty(purchaseComment))
                {
                    column.Item().PaddingTop(15);
                }

                column.Item().ShowOnce().Column(innerColumn =>
                {
                    // Always show header
                    innerColumn.Item().BorderBottom(2).BorderColor("#00873C").PaddingBottom(5)
                        .Text("Installation").FontSize(13).Bold().FontColor("#00873C");

                    // Show content box (empty if no comment text)
                    innerColumn.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                        .Text(installationComment ?? "").FontSize(9);
                });
            });
        }

        // Data loading methods
        private async Task<InstallationData> LoadBdrDataAsync(Guid workflowId)
        {
            var data = new InstallationData
            {
                Workflow = await _dbContext.Workflow
                    .Include(w => w.Project)
                    .FirstOrDefaultAsync(w => w.Id == workflowId)
            };

            if (data.Workflow != null)
            {
                data.Project = data.Workflow.Project;
            }

            data.BdrHardware = await _dbContext.HardwareComputer
                .FirstOrDefaultAsync(h => h.WorkflowId == workflowId);

            if (data.BdrHardware != null)
            {
                data.HardwareFields = await _dbContext.HardwareField
                    .Where(f => f.HardwareComputerId == data.BdrHardware.Id && f.IsActive)
                    .OrderBy(f => f.DisplayOrder)
                    .ToListAsync();
            }

            data.InstallationConfig = await _dbContext.InstallationConfiguration
                .FirstOrDefaultAsync(c => c.WorkflowId == workflowId);

            return data;
        }

        private async Task<InstallationData> LoadBvDataAsync(Guid workflowId)
        {
            var data = new InstallationData
            {
                Workflow = await _dbContext.Workflow
                    .Include(w => w.Project)
                    .FirstOrDefaultAsync(w => w.Id == workflowId)
            };

            if (data.Workflow != null)
            {
                data.Project = data.Workflow.Project;
            }

            data.BvHardware = await _dbContext.BvHardwareComputer
                .FirstOrDefaultAsync(h => h.WorkflowId == workflowId);

            if (data.BvHardware != null)
            {
                data.BvHardwareComponents = await _dbContext.BvHardwareComponent
                    .Where(c => c.BvHardwareComputerId == data.BvHardware.Id && c.IsActive)
                    .ToListAsync();
            }

            data.InstallationConfig = await _dbContext.InstallationConfiguration
                .FirstOrDefaultAsync(c => c.WorkflowId == workflowId);

            return data;
        }

        // Helper methods
        private List<SoftwareInfo> ParseSoftwareFromDescription(string description, bool isBdr)
        {
            var result = new List<SoftwareInfo>();
            string prefix = isBdr ? "BDRSOFTWARE:" : "BVSOFTWARE:";

            if (description.Contains(prefix))
            {
                var startIndex = description.IndexOf(prefix) + prefix.Length;
                var endIndex = description.IndexOf("ENDMEC:", startIndex);

                string softwareData = endIndex > startIndex
                    ? description.Substring(startIndex, endIndex - startIndex)
                    : description.Substring(startIndex);

                var entries = softwareData.Split(';');
                foreach (var entry in entries)
                {
                    if (!string.IsNullOrEmpty(entry))
                    {
                        var parts = entry.Split('|');
                        result.Add(new SoftwareInfo
                        {
                            Name = parts.Length > 0 ? parts[0].Replace("PIPE", "|").Replace("SEMICOLON", ";") : "",
                            Version = parts.Length > 1 ? parts[1].Replace("PIPE", "|").Replace("SEMICOLON", ";") : "",
                            HasLicense = isBdr && parts.Length > 2 && parts[2].Equals("true", StringComparison.OrdinalIgnoreCase)
                        });
                    }
                }
            }

            return result;
        }

        private string ParseSoftwareRemarksFromDescription(string description, bool isBdr)
        {
            string prefix = isBdr ? "BDRSOFTWARE_REMARKS:" : "BVSOFTWARE_REMARKS:";

            if (string.IsNullOrEmpty(description) || !description.Contains(prefix))
                return "";

            var startIndex = description.IndexOf(prefix) + prefix.Length;
            var endIndex = description.IndexOf("ENDMEC:", startIndex);

            if (endIndex > startIndex)
            {
                return description.Substring(startIndex, endIndex - startIndex);
            }
            else if (startIndex < description.Length)
            {
                return description.Substring(startIndex);
            }

            return "";
        }

        private string ParseCommentFromDescription(string description, string prefix)
        {
            if (string.IsNullOrEmpty(description) || !description.Contains(prefix))
                return "";

            var startIndex = description.IndexOf(prefix) + prefix.Length;
            var endIndex = description.IndexOf("ENDMEC:", startIndex);

            if (endIndex > startIndex)
            {
                return description.Substring(startIndex, endIndex - startIndex);
            }
            else if (startIndex < description.Length)
            {
                return description.Substring(startIndex);
            }

            return "";
        }

        // Data classes
        private class InstallationData
        {
            public DbWorkflow? Workflow { get; set; }
            public MECWeb.DbModels.Project.DbProject? Project { get; set; }
            public DbHardwareComputer? BdrHardware { get; set; }
            public DbBvHardwareComputer? BvHardware { get; set; }
            public List<DbHardwareField>? HardwareFields { get; set; }
            public List<DbBvHardwareComponent>? BvHardwareComponents { get; set; }
            public DbInstallationConfiguration? InstallationConfig { get; set; }
        }

        private class SoftwareInfo
        {
            public string Name { get; set; } = string.Empty;
            public string? Version { get; set; }
            public bool HasLicense { get; set; }
        }
    }
}