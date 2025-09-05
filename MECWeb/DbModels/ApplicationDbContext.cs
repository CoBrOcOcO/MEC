using MECWeb.DbModels.Project;
using MECWeb.DbModels.Translate;
using MECWeb.DbModels.User;
using MECWeb.DbModels.Workflow;
using MECWeb.DbModels.Machine;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace MECWeb.DbModels
{
    /// <summary>
    /// Database schema creation
    /// </summary>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // User Tables
        public DbSet<DbUser> User { get; set; }
        public DbSet<DbUserProject> UserProject { get; set; }
        public DbSet<DbUserProjectFavorite> UserProjectFavorite { get; set; }

        // Project Tables
        public DbSet<DbProject> Project { get; set; }

        // Machine Tables 
        public DbSet<DbMachine> Machine { get; set; }

        // Translate Tables
        public DbSet<DbTranslateProject> TranslateProject { get; set; }
        public DbSet<DbTranslateEntry> TranslateEntry { get; set; }
        public DbSet<DbTranslateTranslation> TranslateTranslation { get; set; }
        public DbSet<DbTranslateDictionary> TranslateDictionary { get; set; }

        // Workflow & Hardware Tables
        public DbSet<DbWorkflow> Workflow { get; set; }
        public DbSet<DbHardwareComputer> HardwareComputer { get; set; }
        public DbSet<DbBvHardwareComputer> BvHardwareComputer { get; set; }
        public DbSet<DbBvHardwareComponent> BvHardwareComponent { get; set; }
        public DbSet<DbSoftwareComponent> SoftwareComponent { get; set; }
        public DbSet<DbHardwareField> HardwareField { get; set; }
        public DbSet<DbConfigurationEntry> ConfigurationEntry { get; set; }

        // Installation Management Tables
        public DbSet<DbInstallationConfiguration> InstallationConfiguration { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DbUserProject entity (Many-to-Many relationship)
            modelBuilder.Entity<DbUserProject>()
                .HasKey(up => new { up.DbUserId, up.DbProjectId });
            modelBuilder.Entity<DbUserProject>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserProjects)
                .HasForeignKey(up => up.DbUserId);
            modelBuilder.Entity<DbUserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.UserProjects)
                .HasForeignKey(up => up.DbProjectId);

            // Configure DbUserProjectFavorite entity (Many-to-Many relationship)
            modelBuilder.Entity<DbUserProjectFavorite>()
                .HasKey(up => new { up.DbUserId, up.DbProjectId });
            modelBuilder.Entity<DbUserProjectFavorite>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserProjectFavorites)
                .HasForeignKey(up => up.DbUserId);
            modelBuilder.Entity<DbUserProjectFavorite>()
                .HasOne(up => up.Project)
                .WithMany(p => p.UserProjectFavorites)
                .HasForeignKey(up => up.DbProjectId);

            // *** CASCADE DELETE CONFIGURATION ***

            // Workflow relationships
            modelBuilder.Entity<DbWorkflow>()
                .HasOne(w => w.Project)
                .WithMany(p => p.Workflows)
                .HasForeignKey(w => w.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // BDR Hardware Computer - CASCADE DELETE
            modelBuilder.Entity<DbHardwareComputer>()
                .HasOne(hc => hc.Workflow)
                .WithMany(w => w.HardwareComputers)
                .HasForeignKey(hc => hc.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // BDR Hardware Fields - CASCADE DELETE
            modelBuilder.Entity<DbHardwareField>()
                .HasOne(hf => hf.HardwareComputer)
                .WithMany(hc => hc.HardwareFields)
                .HasForeignKey(hf => hf.HardwareComputerId)
                .OnDelete(DeleteBehavior.Cascade);

            // BDR Software Components - CASCADE DELETE
            modelBuilder.Entity<DbSoftwareComponent>()
                .HasOne(sc => sc.HardwareComputer)
                .WithMany(hc => hc.SoftwareComponents)
                .HasForeignKey(sc => sc.HardwareComputerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration Entries - CASCADE DELETE
            modelBuilder.Entity<DbConfigurationEntry>()
                .HasOne(ce => ce.SoftwareComponent)
                .WithMany(sc => sc.ConfigurationEntries)
                .HasForeignKey(ce => ce.SoftwareComponentId)
                .OnDelete(DeleteBehavior.Cascade);

            // BV Hardware Computer - NO ACTION to prevent cascade cycles
            modelBuilder.Entity<DbBvHardwareComputer>()
                .HasOne(bhc => bhc.Workflow)
                .WithMany()
                .HasForeignKey(bhc => bhc.WorkflowId)
                .OnDelete(DeleteBehavior.NoAction);

            // BV Hardware Component - CASCADE DELETE (safe path)
            modelBuilder.Entity<DbBvHardwareComponent>()
                .HasOne(bhc => bhc.BvHardwareComputer)
                .WithMany(bhc => bhc.HardwareComponents)
                .HasForeignKey(bhc => bhc.BvHardwareComputerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Installation Configuration - CASCADE DELETE
            modelBuilder.Entity<DbInstallationConfiguration>()
                .HasOne(ic => ic.Workflow)
                .WithMany()
                .HasForeignKey(ic => ic.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Installation Configuration Index (unique per workflow)
            modelBuilder.Entity<DbInstallationConfiguration>()
                .HasIndex(ic => ic.WorkflowId)
                .IsUnique()
                .HasDatabaseName("IX_InstallationConfiguration_WorkflowId");

            // Additional Indexes for performance
            modelBuilder.Entity<DbWorkflow>()
                .HasIndex(w => new { w.ProjectId, w.Status })
                .HasDatabaseName("IX_Workflow_ProjectId_Status");

            modelBuilder.Entity<DbProject>()
                .HasIndex(p => p.ProjectNumber)
                .IsUnique()
                .HasDatabaseName("IX_Project_ProjectNumber");
        }
    }
}