namespace MECWeb.Services
{
    public class PdfStorageService
    {
        private readonly string _basePath;
        private readonly ILogger<PdfStorageService> _logger;

        public PdfStorageService(IConfiguration configuration, ILogger<PdfStorageService> logger)
        {
            _basePath = configuration.GetValue<string>("PdfStorageFolder")
                ?? Path.Combine(Directory.GetCurrentDirectory(), "Storage", "PDFs");
            _logger = logger;

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> StorePdfAsync(Guid workflowId, byte[] pdfData, string fileName)
        {
            try
            {
                var workflowFolder = Path.Combine(_basePath, workflowId.ToString());
                if (!Directory.Exists(workflowFolder))
                {
                    Directory.CreateDirectory(workflowFolder);
                }

                var filePath = Path.Combine(workflowFolder, fileName);
                await File.WriteAllBytesAsync(filePath, pdfData);

                _logger.LogInformation("PDF stored successfully: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing PDF for workflow {WorkflowId}", workflowId);
                throw;
            }
        }

        public async Task<byte[]?> GetPdfAsync(Guid workflowId, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_basePath, workflowId.ToString(), fileName);

                if (!File.Exists(filePath))
                {
                    return null;
                }

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PDF for workflow {WorkflowId}", workflowId);
                return null;
            }
        }

        public bool PdfExists(Guid workflowId, string fileName)
        {
            var filePath = Path.Combine(_basePath, workflowId.ToString(), fileName);
            return File.Exists(filePath);
        }

        public bool DeletePdf(Guid workflowId, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_basePath, workflowId.ToString(), fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PDF for workflow {WorkflowId}", workflowId);
                return false;
            }
        }
    }
}