using MECWeb.DbModels;
using MECWeb.DbModels.Workflow;
using Microsoft.EntityFrameworkCore;

namespace MECWeb.Services
{
    /// <summary>
    /// Service for managing workflow corrections
    /// </summary>
    public class WorkflowCorrectionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<WorkflowCorrectionService> _logger;

        public WorkflowCorrectionService(
            ApplicationDbContext dbContext,
            ILogger<WorkflowCorrectionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Clear correction flag when the correction has been addressed
        /// </summary>
        public async Task<bool> ClearCorrectionAsync(Guid workflowId)
        {
            try
            {
                var workflow = await _dbContext.Workflow.FindAsync(workflowId);

                if (workflow == null)
                {
                    _logger.LogWarning("Workflow {WorkflowId} not found for clearing correction", workflowId);
                    return false;
                }

                workflow.HasPendingCorrection = false;
                workflow.CorrectionPhase = null;
                workflow.CorrectionNote = null;
                workflow.CorrectionRequestedAt = null;
                workflow.CorrectionRequestedBy = null;
                workflow.LastChange = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Correction cleared for workflow {WorkflowId}", workflowId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing correction for workflow {WorkflowId}", workflowId);
                return false;
            }
        }

        /// <summary>
        /// Check if a workflow has a pending correction
        /// </summary>
        public async Task<bool> HasPendingCorrectionAsync(Guid workflowId)
        {
            try
            {
                var workflow = await _dbContext.Workflow
                    .AsNoTracking()
                    .FirstOrDefaultAsync(w => w.Id == workflowId);

                return workflow?.HasPendingCorrection ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking correction status for workflow {WorkflowId}", workflowId);
                return false;
            }
        }
    }
}