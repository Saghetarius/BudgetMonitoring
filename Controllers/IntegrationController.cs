using Microsoft.AspNetCore.Mvc;
using BudgetMonitoring.Data;
using BudgetMonitoring.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetMonitoring.Controllers
{
    public class IntegrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IntegrationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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

            var model = new IntegrationViewModel
            {
                UtilityCategories = new List<string> { "Свет", "Газ", "Вода", "Интернет", "Отопление" },
                Companies = new List<string> { "Компания A", "Компания B", "Компания C", "Компания D", "Компания E" }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PayUtility(string category, decimal amount)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Устанавливаем TotalBudget в 0 и отнимаем сумму расходов
            var transaction = new BudgetStatusModel
            {
                UserId = user.Id,
                ExpenseAmount = amount,
                Category = category,
                TransactionDate = DateTime.Now,
                TotalBudget = 0 - amount  // Вычитаем сумму расходов
            };

            _context.BudgetStatusModels.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> InvestInCompany(string company, decimal amount)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Устанавливаем TotalBudget в 0 и отнимаем сумму инвестиций
            var investment = new BudgetStatusModel
            {
                UserId = user.Id,
                AnnualInvestments = amount,
                Category = $"Инвестиция: {company}",
                TransactionDate = DateTime.Now,
                TotalBudget = 0 - amount  // Вычитаем сумму инвестиций
            };

            _context.BudgetStatusModels.Add(investment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
