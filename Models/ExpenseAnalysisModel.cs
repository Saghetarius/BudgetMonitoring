using Microsoft.AspNetCore.Mvc;

namespace BudgetMonitoring.Models
{
    public class ExpenseAnalysisModel
    {
        public decimal CurrentMonthIncome { get; set; }
        public decimal CurrentMonthExpenses { get; set; }
        public decimal PreviousMonthIncome { get; set; }
        public decimal PreviousMonthExpenses { get; set; }

        public List<CategoryReport> YearlyReport { get; set; }
        public List<CategoryReport> MonthlyReport { get; set; }
        public List<ExpenseRecord> TransactionHistory { get; set; }

        // Конструктор для инициализации списков по умолчанию
        public ExpenseAnalysisModel()
        {
            YearlyReport = new List<CategoryReport>();
            MonthlyReport = new List<CategoryReport>();
            TransactionHistory = new List<ExpenseRecord>();
        }

        public class CategoryReport
        {
            public string Category { get; set; }
            public decimal TotalAmount { get; set; }
        }

        public class ExpenseRecord
        {
            public decimal Amount { get; set; }
            public string Category { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
        }
    }
}

