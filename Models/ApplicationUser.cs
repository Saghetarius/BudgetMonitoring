using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace BudgetMonitoring.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }  
    }
}
