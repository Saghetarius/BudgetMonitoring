using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace BudgetMonitoring.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Добавьте дополнительные свойства для пользователей, если нужно
        public string FullName { get; set; }  // Пример свойства, которое можно добавить
    }
}
