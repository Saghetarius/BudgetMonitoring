using Microsoft.AspNetCore.Mvc;
using BudgetMonitoring.Models;
using BudgetMonitoring.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BudgetMonitoring.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;  // Используем IdentityUser

        public TransactionController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Страница для ввода доходов и расходов
        public async Task<IActionResult> RecordIncomeExpense()
        {
            var user = await _userManager.GetUserAsync(User); // Получаем текущего пользователя
            var model = new BudgetStatusModel
            {
                UserId = user?.Id // Присваиваем UserId
            };
            return View(model);
        }
        // Обработка формы для ввода доходов и расходов
        [HttpPost]
        public async Task<IActionResult> RecordIncomeExpense(BudgetStatusModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);

                    // Проверка на пустые значения доходов и расходов
                    if (model.IncomeAmount == 0 && model.ExpenseAmount == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Пожалуйста, введите сумму доходов или расходов.");
                        return View(model);
                    }

                    // Если выбрано сохранение 10% на сберегательный счет
                    if (model.Save10Percent && model.IncomeAmount > 0)
                    {
                        model.SavingsAmount = model.IncomeAmount * 0.10M;
                        model.SavingsAccount += model.SavingsAmount;  // Обновляем сберегательный счет
                    }

                    // Обновляем бюджет
                    model.TotalBudget += model.IncomeAmount - model.ExpenseAmount;

                    // Связываем модель с пользователем
                    model.UserId = user?.Id;

                    // Добавляем модель в контекст
                    _context.BudgetStatusModels.Add(model);
                    await _context.SaveChangesAsync();

                    // Перенаправление на страницу с общим бюджетом
                    return RedirectToAction("Index", "BudgetStatus");
                }
                catch (Exception ex)
                {
                    // Логируем ошибку
                    Console.WriteLine($"Ошибка при сохранении в базу данных: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Ошибка при сохранении данных.");
                }
            }

            return View(model);
        }

    }
}
