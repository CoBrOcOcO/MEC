using MECWeb.DbModels.Project;
using MECWeb.DbModels.Workflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MECWeb.Services
{
    public class EnhancedProjectService
    {
        private readonly ILogger<EnhancedProjectService> _logger;
        private readonly IConfiguration _configuration;

        public EnhancedProjectService(
            ILogger<EnhancedProjectService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task<bool> CreateProjectAsync(string projectName)
        {
            try
            {
                _logger.LogInformation($"Creating project: {projectName}");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating project {projectName}");
                return Task.FromResult(false);
            }
        }

        public Task<List<DbWorkflow>> GetWorkflowsAsync(Guid projectId)
        {
            var workflows = new List<DbWorkflow>
            {
                new DbWorkflow { Id = Guid.NewGuid(), Name = "Setup", WorkflowType = WorkflowType.BDR, ProjectId = projectId },
                new DbWorkflow { Id = Guid.NewGuid(), Name = "Development", WorkflowType = WorkflowType.BV, ProjectId = projectId }
            };

            return Task.FromResult(workflows);
        }
    }
}