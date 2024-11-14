using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BudgetMonitoring.Models
{
    public class IntegrationViewModel
    {
        // Список категорий коммунальных услуг
        public List<string> UtilityCategories { get; set; }

        // Список компаний для инвестиций
        public List<string> Companies { get; set; }
        public BudgetStatusModel BudgetStatus { get; set; }
    }
}
