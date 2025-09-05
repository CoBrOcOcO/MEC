using MECWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MECWeb.Controllers
{
    [ApiController]
    [Route("api/pdf")]
    public class PdfController : ControllerBase
    {
        private readonly InstallationPdfService _pdfService;
        private readonly PdfStorageService _pdfStorageService;
        private readonly ILogger<PdfController> _logger;

        public PdfController(
            InstallationPdfService pdfService,
            PdfStorageService pdfStorageService,
            ILogger<PdfController> logger)
        {
            _pdfService = pdfService;
            _pdfStorageService = pdfStorageService;
            _logger = logger;
        }

        [HttpGet("download/{workflowId}/{type}")]
        public async Task<IActionResult> DownloadSinglePdf(Guid workflowId, string type)
        {
            try
            {
                PdfResult result;
                if (type.Equals("BDR", StringComparison.OrdinalIgnoreCase))
                {
                    result = await _pdfService.GenerateBdrPdfAsync(workflowId);
                }
                else if (type.Equals("BV", StringComparison.OrdinalIgnoreCase))
                {
                    result = await _pdfService.GenerateBvPdfAsync(workflowId);
                }
                else
                {
                    return BadRequest("Invalid PDF type specified");
                }

                if (!result.IsSuccess || result.PdfData == null || string.IsNullOrEmpty(result.FileName))
                {
                    return BadRequest(result.ErrorMessage);
                }

                return File(result.PdfData, "application/pdf", result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for workflow {WorkflowId}", workflowId);
                return StatusCode(500, "An error occurred while generating the PDF");
            }
        }

        [HttpGet("download-project/{projectId}/{type}")]
        public async Task<IActionResult> DownloadProjectPdfs(Guid projectId, string type)
        {
            try
            {
                var fileName = $"{type}_Installation_{projectId}_{DateTime.Now:yyyyMMdd}.pdf";
                var storedPdf = await _pdfStorageService.GetPdfAsync(projectId, fileName);

                if (storedPdf != null)
                {
                    return File(storedPdf, "application/pdf", fileName);
                }

                PdfResult result;
                if (type.Equals("BDR", StringComparison.OrdinalIgnoreCase))
                {
                    result = await _pdfService.GenerateAllBdrPdfsAsync(projectId);
                }
                else if (type.Equals("BV", StringComparison.OrdinalIgnoreCase))
                {
                    result = await _pdfService.GenerateAllBvPdfsAsync(projectId);
                }
                else
                {
                    return BadRequest("Invalid PDF type specified");
                }

                if (!result.IsSuccess || result.PdfData == null || string.IsNullOrEmpty(result.FileName))
                {
                    return BadRequest(result.ErrorMessage);
                }

                return File(result.PdfData, "application/pdf", result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDFs for project {ProjectId}", projectId);
                return StatusCode(500, "An error occurred while generating the PDFs");
            }
        }
    }
}
