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

        public InstallationPdfGenerator(ApplicationDbContext dbContext, ILogger<InstallationPdfGenerator> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Generate PDF for BDR Installation
        /// </summary>
        public async Task<byte[]> GenerateBdrInstallationPdfAsync(Guid workflowId)
        {
            var data = await LoadBdrDataAsync(workflowId);

            if (data.Workflow == null)
            {
                throw new InvalidOperationException("Workflow not found");
            }

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Header
                    page.Header()
                        .Background(Colors.Green.Lighten1)
                        .Padding(10)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Installation")
                                    .FontSize(16).FontColor(Colors.White).Bold();
                            });
                            row.ConstantItem(120).Text($"Datum: {DateTime.Now:dd.MM.yyyy}")
                                .FontSize(10).FontColor(Colors.White);
                        });

                    // Content
                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            // Project Information
                            column.Item().Element(c => CreateProjectSection(c, data));
                            column.Item().PaddingTop(15);

                            // Hardware Configuration
                            column.Item().Element(c => CreateBdrHardwareSection(c, data));
                            column.Item().PaddingTop(15);

                            // Software Configuration
                            column.Item().Element(c => CreateSoftwareSection(c, data));
                            column.Item().PaddingTop(15);

                            // Network Configuration
                            column.Item().Element(c => CreateNetworkSection(c, data));
                            column.Item().PaddingTop(15);

                            // Comments
                            column.Item().Element(c => CreateCommentsSection(c, data));
                        });

                    // Footer
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
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
        /// Generate PDF for BV Installation
        /// </summary>
        public async Task<byte[]> GenerateBvInstallationPdfAsync(Guid workflowId)
        {
            var data = await LoadBvDataAsync(workflowId);

            if (data.Workflow == null)
            {
                throw new InvalidOperationException("Workflow not found");
            }

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Header 
                    page.Header()
                        .Background(Colors.Green.Darken1)  
                        .Padding(10)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Installation")
                                    .FontSize(16).FontColor(Colors.White).Bold();
                            });
                            row.ConstantItem(120).Text($"Datum: {DateTime.Now:dd.MM.yyyy}")
                                .FontSize(10).FontColor(Colors.White);
                        });

                    // Content
                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            // Project Information
                            column.Item().Element(c => CreateProjectSection(c, data));
                            column.Item().PaddingTop(15);

                            // Hardware Configuration
                            column.Item().Element(c => CreateBvHardwareSection(c, data));
                            column.Item().PaddingTop(15);

                            // Software Configuration
                            column.Item().Element(c => CreateSoftwareSection(c, data));
                            column.Item().PaddingTop(15);

                            // Network Configuration
                            column.Item().Element(c => CreateNetworkSection(c, data));
                            column.Item().PaddingTop(15);

                            // Comments
                            column.Item().Element(c => CreateCommentsSection(c, data));
                        });

                    // Footer
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
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
        /// Create Project Information Section
        /// </summary>
        private void CreateProjectSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                column.Item().Text("Projekt-Information")
                    .FontSize(14).Bold().FontColor(Colors.Green.Lighten1);

                column.Item().PaddingTop(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(120);
                        columns.RelativeColumn();
                    });

                    table.Cell().Border(1).Padding(5).Text("Projektnummer:").Bold();
                    table.Cell().Border(1).Padding(5).Text(data.Project?.ProjectNumber ?? "-");

                    table.Cell().Border(1).Padding(5).Text("Projektname:").Bold();
                    table.Cell().Border(1).Padding(5).Text(data.Project?.Name ?? "-");

                    table.Cell().Border(1).Padding(5).Text("Standort:").Bold();
                    table.Cell().Border(1).Padding(5).Text(data.Workflow?.WorkflowType.ToString() ?? "-");

                    table.Cell().Border(1).Padding(5).Text("Erstellt am:").Bold();
                    table.Cell().Border(1).Padding(5).Text(data.Workflow?.CreationDate.ToString("dd.MM.yyyy HH:mm") ?? "-");
                });
            });
        }

        /// <summary>
        /// Create BDR Hardware Section
        /// </summary>
        private void CreateBdrHardwareSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                column.Item().Text("Hardware-Konfiguration")
                    .FontSize(14).Bold().FontColor(Colors.Green.Lighten1);

                if (data.BdrHardware != null)
                {
                    column.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(120);
                            columns.RelativeColumn();
                        });

                        table.Cell().Border(1).Padding(5).Text("Rechner-Typ:").Bold();
                        table.Cell().Border(1).Padding(5).Text(data.BdrHardware.Name ?? "-");


                        if (!string.IsNullOrEmpty(data.BdrHardware.Description))
                        {
                            table.Cell().Border(1).Padding(5).Text("Kommentar:").Bold();
                            table.Cell().Border(1).Padding(5).Text(data.BdrHardware.Description);
                        }
                    });

                    // Hardware Fields
                    if (data.HardwareFields?.Any() == true)
                    {
                        column.Item().PaddingTop(10).Text("Hardware-Details:")
                            .FontSize(12).Bold().FontColor(Colors.Green.Lighten1);

                        column.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Zusätzliche Hardware").Bold();
                                header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Wert").Bold();
                            });

                            foreach (var field in data.HardwareFields.Where(f => !string.IsNullOrWhiteSpace(f.FieldValue)))
                            {
                                table.Cell().Border(1).Padding(5).Text(field.DisplayName ?? field.FieldName);
                                table.Cell().Border(1).Padding(5).Text(field.FieldValue ?? "-");
                            }
                        });
                    }
                }
                else
                {
                    column.Item().Text("Keine Hardware-Konfiguration gefunden.").Italic();
                }
            });
        }

        /// <summary>
        /// Create BV Hardware Section
        /// </summary>
        private void CreateBvHardwareSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                column.Item().Text("Hardware-Konfiguration")
                    .FontSize(14).Bold().FontColor(Colors.Green.Darken1);  

                if (data.BvHardware != null)
                {
                    column.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(120);
                            columns.RelativeColumn();
                        });

                        table.Cell().Border(1).Padding(5).Text("Ansprechpartner:").Bold();
                        table.Cell().Border(1).Padding(5).Text(data.BvHardware.ContactPerson ?? "-");

                        table.Cell().Border(1).Padding(5).Text("Rechner-Typ:").Bold();
                        table.Cell().Border(1).Padding(5).Text(data.BvHardware.PcType ?? "-");


                        if (!string.IsNullOrEmpty(data.BvHardware.Description))
                        {
                            table.Cell().Border(1).Padding(5).Text("Kommentar:").Bold();
                            table.Cell().Border(1).Padding(5).Text(data.BvHardware.Description);
                        }
                    });

                    // Hardware Components
                    if (data.BvHardwareComponents?.Any() == true)
                    {
                        column.Item().PaddingTop(10).Text("Hardware-Komponenten:")
                            .FontSize(12).Bold().FontColor(Colors.Blue.Lighten1);

                        column.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(60);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Komponente").Bold();
                                header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Anzahl").Bold();
                                header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Typ").Bold();
                                header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Kategorie").Bold();
                            });

                            foreach (var component in data.BvHardwareComponents.Where(c => c.IsSelected))
                            {
                                table.Cell().Border(1).Padding(5).Text(component.ComponentName ?? "-");
                                table.Cell().Border(1).Padding(5).Text(component.Quantity ?? "1");
                                table.Cell().Border(1).Padding(5).Text(component.ComponentType ?? "-");
                                table.Cell().Border(1).Padding(5).Text(component.Category ?? "-");
                            }
                        });
                    }
                }
                else
                {
                    column.Item().Text("Keine Hardware-Konfiguration gefunden.").Italic();
                }
            });
        }

        /// <summary>
        /// Create Software Section
        /// </summary>
        private void CreateSoftwareSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                column.Item().Text("Software-Konfiguration")
                    .FontSize(14).Bold().FontColor(Colors.Green.Lighten1);

                var softwareList = ParseSoftwareFromDescription(data.Workflow?.Description ?? "", data.Workflow?.WorkflowType == WorkflowType.BDR);

                if (softwareList.Any())
                {
                    column.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Software").Bold();
                            header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Version/Hinweise").Bold();
                            header.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Lizenz").Bold();
                        });

                        foreach (var software in softwareList)
                        {
                            table.Cell().Border(1).Padding(5).Text(software.Name);
                            table.Cell().Border(1).Padding(5).Text(software.Version ?? "-");
                            table.Cell().Border(1).Padding(5).Text(software.HasLicense ? "Ja" : "Nein");
                        }
                    });
                }
                else
                {
                    column.Item().Text("Keine Software konfiguriert.").Italic();
                }
            });
        }

        /// <summary>
        /// Create Network Configuration Section
        /// </summary>
        private void CreateNetworkSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                column.Item().Text("Netzwerk-Konfiguration")
                    .FontSize(14).Bold().FontColor(Colors.Green.Lighten1);

                if (data.InstallationConfig != null)
                {
                    column.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(120);
                            columns.RelativeColumn();
                        });

                        table.Cell().Border(1).Padding(5).Text("MAC-Adresse:").Bold();
                        table.Cell().Border(1).Padding(5).Text(data.InstallationConfig.MacAddress ?? "Nicht konfiguriert");

                        table.Cell().Border(1).Padding(5).Text("IP-Adresse:").Bold();
                        table.Cell().Border(1).Padding(5).Text(data.InstallationConfig.IpAddress ?? "Nicht konfiguriert");


                        if (!string.IsNullOrEmpty(data.InstallationConfig.NetworkNotes))
                        {
                            table.Cell().Border(1).Padding(5).Text("Netzwerk-Hinweise:").Bold();
                            table.Cell().Border(1).Padding(5).Text(data.InstallationConfig.NetworkNotes);
                        }
                    });
                }
                else
                {
                    column.Item().Text("Keine Netzwerk-Konfiguration gefunden.").Italic();
                }
            });
        }

        /// <summary>
        /// Create Comments Section
        /// </summary>
        private void CreateCommentsSection(IContainer container, InstallationData data)
        {
            container.Column(column =>
            {
                column.Item().Text("Kommentare & Hinweise")
                    .FontSize(14).Bold().FontColor(Colors.Green.Lighten1);

                var purchaseComment = ParseCommentFromDescription(data.Workflow?.Description ?? "", "PURCHASE_COMMENT:");
                var installationComment = ParseCommentFromDescription(data.Workflow?.Description ?? "", "INSTALLATION_COMMENT:");

                var hasComments = !string.IsNullOrEmpty(purchaseComment) || !string.IsNullOrEmpty(installationComment);

                if (hasComments)
                {
                    column.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(120);
                            columns.RelativeColumn();
                        });

                        if (!string.IsNullOrEmpty(purchaseComment))
                        {
                            table.Cell().Border(1).Padding(5).Text("Einkauf-Kommentar:").Bold();
                            table.Cell().Border(1).Padding(5).Text(purchaseComment);
                        }

                        if (!string.IsNullOrEmpty(installationComment))
                        {
                            table.Cell().Border(1).Padding(5).Text("Installation-Hinweise:").Bold();
                            table.Cell().Border(1).Padding(5).Text(installationComment);
                        }
                    });
                }
                else
                {
                    column.Item().Text("Keine Kommentare vorhanden.").Italic();
                }
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