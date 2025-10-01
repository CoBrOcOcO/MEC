using MECWeb.DbModels;
using MECWeb.DbModels.Workflow;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using iText.Kernel.Pdf;
using iText.IO.Source;
using System.IO;

namespace MECWeb.Services
{
    public class PdfResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public byte[]? PdfData { get; set; }
        public string? FileName { get; set; }
    }

    public class InstallationPdfService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly InstallationPdfGenerator _pdfGenerator;
       
        private readonly ILogger<InstallationPdfService> _logger;

        public InstallationPdfService(
            ApplicationDbContext dbContext,
            InstallationPdfGenerator pdfGenerator,
         
            ILogger<InstallationPdfService> logger)
        {
            _dbContext = dbContext;
            _pdfGenerator = pdfGenerator; 
            _logger = logger;
        }

        public async Task<PdfResult> GenerateAllBdrPdfsAsync(Guid projectId)
        {
            try
            {
                _logger.LogInformation("Generating all BDR PDFs for project {ProjectId}", projectId);

                var bdrWorkflows = await _dbContext.Workflow
                    .Include(w => w.Project)
                    .Where(w => w.ProjectId == projectId && w.WorkflowType == WorkflowType.BDR)
                    .Where(w => w.Status == WorkflowStatus.ForwardedToInstallation)
                    .ToListAsync();

                if (!bdrWorkflows.Any())
                {
                    return new PdfResult { IsSuccess = false, ErrorMessage = "Keine BDR-Workflows gefunden." };
                }

                var pdfFiles = new List<byte[]>(bdrWorkflows.Count);
                foreach (var workflow in bdrWorkflows)
                {
                    var pdf = await _pdfGenerator.GenerateBdrInstallationPdfAsync(workflow.Id);
                    pdfFiles.Add(pdf);
                }

                _logger.LogInformation("Successfully generated {Count} BDR PDFs", bdrWorkflows.Count);

                var projectNumber = bdrWorkflows.First().Project?.ProjectNumber ?? projectId.ToString();
                return new PdfResult
                {
                    IsSuccess = true,
                    FileName = $"BDR_Installation_{projectNumber}_{DateTime.Now:yyyyMMdd}.pdf",
                    PdfData = CombinePdfs(pdfFiles)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating all BDR PDFs for project {ProjectId}", projectId);
                return new PdfResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<PdfResult> GenerateAllBvPdfsAsync(Guid projectId)
        {
            try
            {
                _logger.LogInformation("Generating all BV PDFs for project {ProjectId}", projectId);

                var bvWorkflows = await _dbContext.Workflow
                    .Include(w => w.Project)
                    .Where(w => w.ProjectId == projectId && w.WorkflowType == WorkflowType.BV)
                    .Where(w => w.Status == WorkflowStatus.ForwardedToInstallation)
                    .ToListAsync();

                if (!bvWorkflows.Any())
                {
                    return new PdfResult { IsSuccess = false, ErrorMessage = "Keine BV-Workflows gefunden." };
                }

                var pdfFiles = new List<byte[]>();
                foreach (var workflow in bvWorkflows)
                {
                    var pdf = await _pdfGenerator.GenerateBvInstallationPdfAsync(workflow.Id);
                    pdfFiles.Add(pdf);
                }

                _logger.LogInformation("Successfully generated {Count} BV PDFs", bvWorkflows.Count);

                var projectNumber = bvWorkflows.First().Project?.ProjectNumber ?? projectId.ToString();
                return new PdfResult
                {
                    IsSuccess = true,
                    FileName = $"BV_Installation_{projectNumber}_{DateTime.Now:yyyyMMdd}.pdf",
                    PdfData = CombinePdfs(pdfFiles)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating all BV PDFs for project {ProjectId}", projectId);
                return new PdfResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<PdfResult> GenerateBdrPdfAsync(Guid workflowId)
        {
            try
            {
                _logger.LogInformation("Generating BDR PDF for workflow {WorkflowId}", workflowId);

                var workflow = await _dbContext.Workflow
                    .Include(w => w.Project)
                    .FirstOrDefaultAsync(w => w.Id == workflowId);

                if (workflow == null)
                {
                    return new PdfResult { IsSuccess = false, ErrorMessage = "Workflow nicht gefunden." };
                }

                if (workflow.WorkflowType != WorkflowType.BDR)
                {
                    return new PdfResult { IsSuccess = false, ErrorMessage = "Workflow ist nicht vom Typ BDR." };
                }

                var pdfData = await _pdfGenerator.GenerateBdrInstallationPdfAsync(workflowId);
                var fileName = $"BDR_Installation_{workflow.Name}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";


                _logger.LogInformation("Successfully generated BDR PDF for workflow {WorkflowId}", workflowId);

                return new PdfResult
                {
                    IsSuccess = true,
                    FileName = fileName,
                    PdfData = pdfData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating BDR PDF for workflow {WorkflowId}", workflowId);
                return new PdfResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<PdfResult> GenerateBvPdfAsync(Guid workflowId)
        {
            try
            {
                _logger.LogInformation("Generating BV PDF for workflow {WorkflowId}", workflowId);

                var workflow = await _dbContext.Workflow
                    .Include(w => w.Project)
                    .FirstOrDefaultAsync(w => w.Id == workflowId);

                if (workflow == null)
                {
                    return new PdfResult { IsSuccess = false, ErrorMessage = "Workflow nicht gefunden." };
                }

                if (workflow.WorkflowType != WorkflowType.BV)
                {
                    return new PdfResult { IsSuccess = false, ErrorMessage = "Workflow ist nicht vom Typ BV." };
                }

                var pdfData = await _pdfGenerator.GenerateBvInstallationPdfAsync(workflowId);
                var fileName = $"BV_Installation_{workflow.Name}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                _logger.LogInformation("Successfully generated BV PDF for workflow {WorkflowId}", workflowId);

                return new PdfResult
                {
                    IsSuccess = true,
                    FileName = fileName,
                    PdfData = pdfData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating BV PDF for workflow {WorkflowId}", workflowId);
                return new PdfResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        /// <summary>
        /// Combine multiple PDFs using iText7
        /// </summary>
        private byte[] CombinePdfs(List<byte[]> pdfFiles)
        {
            if (!pdfFiles.Any())
                return [];
            
            if (pdfFiles.Count == 1)
                return pdfFiles[0];

            using var ms = new MemoryStream();
            using (var writer = new PdfWriter(ms))
            using (var mergedPdf = new PdfDocument(writer))
            {
                foreach (var pdfBytes in pdfFiles)
                {
                    using var reader = new PdfReader(new MemoryStream(pdfBytes));
                    using var sourcePdf = new PdfDocument(reader);
                    sourcePdf.CopyPagesTo(1, sourcePdf.GetNumberOfPages(), mergedPdf);
                }
            }

            return ms.ToArray();
        }
    }
}