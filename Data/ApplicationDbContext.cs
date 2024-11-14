using BudgetMonitoring.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetMonitoring.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BudgetStatusModel> BudgetStatusModels { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<DebtCredit> DebtCredits { get; set; }
        public DbSet<BudgetStatusModel> BudgetStatus { get; set; }
    }
}
