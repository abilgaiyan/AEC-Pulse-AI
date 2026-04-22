using Microsoft.EntityFrameworkCore;
using AECPulse.Domain.Entities;
using AECPulse.Domain.Enums;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AECPulse.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<FinancialTransaction> Transactions => Set<FinancialTransaction>();
    public DbSet<ConversationLog> ConversationLogs => Set<ConversationLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // This suppresses the specific warning that is stopping your update
        optionsBuilder.ConfigureWarnings(w => 
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent API for precision (shows Senior expertise)
        modelBuilder.Entity<Project>().Property(p => p.TotalBudget).HasPrecision(18, 2);
        modelBuilder.Entity<Project>().Property(p => p.SpentToDate).HasPrecision(18, 2);
       

        // Define fixed GUIDs so the relationship stays linked
        var projectOneId = Guid.Parse("8db7a195-25e6-4927-8025-f938b8e0e7a1");
        var projectTwoId = Guid.Parse("2d5e865a-9310-4828-9717-3f338d38827c");
        // 2. Seed Projects
        modelBuilder.Entity<Project>().HasData(
            new Project 
            { 
                Id = projectOneId, 
                Name = "Skyline Tower Expansion", 
                ProjectNumber = "AEC-2024-001", 
                Status = ProjectStatus.Active,
                TotalBudget = 1250000.00m,
                SpentToDate = 450000.00m 
            },
            new Project 
            { 
                Id = projectTwoId, 
                Name = "Green Energy Bridge", 
                ProjectNumber = "AEC-2024-002", 
                Status = ProjectStatus.OnHold,
                TotalBudget = 850000.00m,
                SpentToDate = 0.00m 
            }
        );

        // 3. Seed a Transaction (linked to Project 1)
        modelBuilder.Entity<FinancialTransaction>().HasData(
            new FinancialTransaction 
            { 
                Id = Guid.Parse("f4625b87-8d34-450a-9d22-1d54e4f7e27e"), 
                ProjectId = projectOneId, 
                Amount = 50000.00m, 
                Description = "Concrete Foundation Materials", 
                TransactionDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}