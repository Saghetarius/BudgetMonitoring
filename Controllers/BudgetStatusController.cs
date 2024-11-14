using BudgetMonitoring.Data;
using BudgetMonitoring.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class BudgetStatusController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public BudgetStatusController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Получаем все записи по бюджету для пользователя
        var budgetData = _context.BudgetStatus
            .Where(b => b.UserId == user.Id)
            .OrderBy(b => b.TransactionDate) 
            .ToList(); 

        // Получаем данные о расходах
        var totalBudget = budgetData.Sum(b => b.TotalBudget); 

        // Расходы за текущий месяц
        var currentMonth = DateTime.Now.Month;
        var monthlyExpenses = budgetData
            .Where(t => t.TransactionDate.Month == currentMonth)
            .Sum(t => t.ExpenseAmount); 

        // Получаем данные о годовых расходах
        var annualExpenses = budgetData
            .Where(t => t.TransactionDate.Year <= DateTime.Now.Year) 
            .Sum(t => t.ExpenseAmount); 

        // Получаем данные о долгах и кредитах
        var totalDebts = await _context.DebtCredits
            .Where(d => d.UserId == user.Id && d.IsDebt == true && !d.IsPaid)
            .SumAsync(d => d.Amount);

        var totalCredits = await _context.DebtCredits
            .Where(c => c.UserId == user.Id && c.IsDebt == false && !c.IsPaid)
            .SumAsync(c => c.Amount);

        // Получаем данные об инвестициях
        var annualInvestments = _context.BudgetStatus
            .Where(i => i.UserId == user.Id)
            .Sum(i => i.AnnualInvestments);

        // Получаем данные о сбережениях
        var savingsAccount = _context.BudgetStatus
            .Where(s => s.UserId == user.Id)
            .Sum(s => s.SavingsAccount);

        // Логика для получения данных об изменении бюджета по месяцам и годам
        var budgetChanges = budgetData
            .GroupBy(b => new { b.TransactionDate.Month, b.TransactionDate.Year })
            .Select(g => new
            {
                Month = g.Key.Month,
                Year = g.Key.Year,
                BudgetAtMoment = g.LastOrDefault().TotalBudget
            })
            .OrderBy(b => b.Year)
            .ThenBy(b => b.Month)
            .ToList();

        // Преобразуем данные для передачи в представление
        var months = budgetChanges.Select(b => $"{b.Year}-{b.Month:D2}").ToArray();
        var budgets = budgetChanges.Select(b => b.BudgetAtMoment).ToArray();

        // Формируем модель
        var budgetStatusModel = new BudgetStatusModel
        {
            TotalBudget = totalBudget,
            MonthlyExpenses = monthlyExpenses,
            AnnualExpenses = annualExpenses,
            TotalDebt = totalDebts,
            CreditDebt = totalCredits,
            AnnualInvestments = annualInvestments,
            SavingsAccount = savingsAccount
        };

        // Передаем данные для графика и показателей на страницу
        ViewBag.Months = months;
        ViewBag.Budgets = budgets;

        return View(budgetStatusModel);
    }
}
