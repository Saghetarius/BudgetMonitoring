using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetMonitoring.Models
{
    public class BudgetStatusModel
    {
        // Свойства для первой страницы "Статус бюджета"
        public decimal TotalBudget { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal AnnualExpenses { get; set; }
        public decimal TotalDebt { get; set; }
        public decimal CreditDebt { get; set; }
        public decimal AnnualInvestments { get; set; }

        // Свойства для второй страницы "Учет расходов и доходов"
        public int Id { get; set; }
        public decimal IncomeAmount { get; set; }
        public decimal ExpenseAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Category { get; set; }
        public bool Save10Percent { get; set; }  
        public decimal SavingsAmount { get; set; }  
        public decimal SavingsAccount { get; set; }  
        public string UserId { get; set; }

        // Добавленные свойства для целей
        public List<Goal> Goals { get; set; } = new List<Goal>(); 
    }


    // Модель для целей
    public class Goal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public string UserId { get; set; }
        public bool IsCompleted { get; set; }

    }

    public class GoalViewModel
    {
        public List<Goal> ActiveGoals { get; set; } 
        public List<Goal> CompletedGoals { get; set; } 
        public Goal Goal { get; set; }
    }

    public class DebtCredit
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }  
        public decimal InterestRate { get; set; }  
        public DateTime DueDate { get; set; }  
        public string UserId { get; set; }  
        public bool IsDebt { get; set; }  
        public bool IsPaid { get; set; } 
    }
    namespace BudgetMonitoring.Models
    {
        public class BudgetStatus
        {
            public int Id { get; set; }
            public decimal TotalBudget { get; set; }
            public decimal MonthlyExpenses { get; set; }
            public decimal AnnualExpenses { get; set; }
            public decimal TotalDebt { get; set; }
            public decimal CreditDebt { get; set; }
            public decimal AnnualInvestments { get; set; }
            public decimal IncomeAmount { get; set; }
            public decimal ExpenseAmount { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Category { get; set; }
            public bool Save10Percent { get; set; }
            public decimal SavingsAmount { get; set; }
            public decimal SavingsAccount { get; set; }
            public string UserId { get; set; }
        }
    }


}
