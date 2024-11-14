using Microsoft.AspNetCore.Mvc;
using BudgetMonitoring.Models;
using BudgetMonitoring.Data;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BudgetMonitoring.Controllers
{
    public class ExpenseAnalysisController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ExpenseAnalysisController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Страница для анализа расходов
        public async Task<IActionResult> Analysis()
        {
            var user = await _userManager.GetUserAsync(User); 
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var currentMonth = DateTime.Now.Month;
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var currentYear = DateTime.Now.Year;

            var currentMonthExpenses = _context.BudgetStatusModels
                .Where(b => b.UserId == user.Id && b.TransactionDate.Month == currentMonth && b.TransactionDate.Year == currentYear)
                .ToList();
            var previousMonthExpenses = _context.BudgetStatusModels
                .Where(b => b.UserId == user.Id && b.TransactionDate.Month == previousMonth && b.TransactionDate.Year == currentYear)
                .ToList();

            if (!currentMonthExpenses.Any())
            {
                Console.WriteLine("Нет данных для текущего месяца");
            }
            if (!previousMonthExpenses.Any())
            {
                Console.WriteLine("Нет данных для предыдущего месяца");
            }

            var expenseAnalysisModel = new ExpenseAnalysisModel
            {
                CurrentMonthIncome = currentMonthExpenses.Where(e => e.IncomeAmount > 0).Sum(e => e.IncomeAmount),
                CurrentMonthExpenses = currentMonthExpenses.Where(e => e.ExpenseAmount > 0).Sum(e => e.ExpenseAmount),
                PreviousMonthIncome = previousMonthExpenses.Where(e => e.IncomeAmount > 0).Sum(e => e.IncomeAmount),
                PreviousMonthExpenses = previousMonthExpenses.Where(e => e.ExpenseAmount > 0).Sum(e => e.ExpenseAmount),
                // Годовой отчет
                YearlyReport = _context.BudgetStatusModels
                    .Where(b => b.UserId == user.Id && b.TransactionDate.Year == currentYear)
                    .GroupBy(b => b.Category)
                    .Select(g => new ExpenseAnalysisModel.CategoryReport
                    {
                        Category = g.Key,
                        TotalAmount = g.Sum(x => x.IncomeAmount + x.ExpenseAmount)
                    }).ToList(),

                // Месячный отчет
                MonthlyReport = _context.BudgetStatusModels
                    .Where(b => b.UserId == user.Id && b.TransactionDate.Month == currentMonth && b.TransactionDate.Year == currentYear)
                    .GroupBy(b => b.Category)
                    .Select(g => new ExpenseAnalysisModel.CategoryReport
                    {
                        Category = g.Key,
                        TotalAmount = g.Sum(x => x.IncomeAmount + x.ExpenseAmount)
                    }).ToList(),

                // История транзакций
                TransactionHistory = _context.BudgetStatusModels
                    .Where(b => b.UserId == user.Id)
                    .Select(b => new ExpenseAnalysisModel.ExpenseRecord
                    {
                        Amount = b.IncomeAmount + b.ExpenseAmount,
                        Category = b.Category,
                        TransactionDate = b.TransactionDate,
                        Description = b.Save10Percent ? "Переведено на сберегательный счет" : "Обычная транзакция"
                    }).ToList()
            };

            return View(expenseAnalysisModel);  
        }



        // Экспорт данных в CSV
        public IActionResult ExportToCsv()
        {
            var records = _context.BudgetStatusModels
                .Select(b => new ExpenseAnalysisModel.ExpenseRecord
                {
                    Amount = b.IncomeAmount + b.ExpenseAmount,
                    Category = b.Category,
                    TransactionDate = b.TransactionDate,
                    Description = b.Save10Percent ? "Переведено на сберегательный счет" : "Обычная транзакция"
                }).ToList();

            var csv = "Amount,Category,Date,Description\n";
            foreach (var record in records)
            {
                csv += $"{record.Amount},{record.Category},{record.TransactionDate.ToString("yyyy-MM-dd")},{record.Description}\n";
            }

            var fileName = "TransactionHistory.csv";
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        }
    }
}
