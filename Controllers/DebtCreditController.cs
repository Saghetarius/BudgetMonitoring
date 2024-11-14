using Microsoft.AspNetCore.Mvc;
using BudgetMonitoring.Data;
using BudgetMonitoring.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace BudgetMonitoring.Controllers
{
    public class DebtCreditController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DebtCreditController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Страница управления долгами и кредитами
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var debtsCredits = _context.DebtCredits.Where(dc => dc.UserId == user.Id).ToList();

            return View(debtsCredits);
        }

        // Страница для добавления долга
        public IActionResult AddDebt()
        {
            return View();
        }

        // Страница для добавления кредита
        public IActionResult AddCredit()
        {
            return View();
        }

        // Метод для добавления долга или кредита
        [HttpPost]
        public async Task<IActionResult> AddDebtCredit(decimal amount, decimal interestRate, DateTime dueDate, bool isDebt)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var debtCredit = new DebtCredit
            {
                Amount = amount,
                InterestRate = isDebt ? 0 : interestRate,  // если долг, процент не нужен
                DueDate = dueDate,
                IsDebt = isDebt,
                UserId = user.Id,
                IsPaid = false
            };

            _context.DebtCredits.Add(debtCredit);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Метод для удаления долга или кредита
        [HttpPost]
        public async Task<IActionResult> DeleteDebtCredit(int id)
        {
            var debtCredit = await _context.DebtCredits.FindAsync(id);
            if (debtCredit != null)
            {
                _context.DebtCredits.Remove(debtCredit);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Метод для расчета графика погашения кредита
        public IActionResult PaymentSchedule(int debtCreditId)
        {
            var debtCredit = _context.DebtCredits.FirstOrDefault(dc => dc.Id == debtCreditId);
            if (debtCredit == null || debtCredit.IsDebt)
            {
                return RedirectToAction("Index");
            }

            var schedule = CalculateLoanSchedule(debtCredit.Amount, debtCredit.InterestRate, debtCredit.DueDate);
            return View(schedule);
        }

        // Метод для уведомления о ближайшей оплате кредита
        public IActionResult NotifyUpcomingPayment(int debtCreditId)
        {
            var debtCredit = _context.DebtCredits.FirstOrDefault(dc => dc.Id == debtCreditId);
            if (debtCredit != null && !debtCredit.IsDebt)
            {
                if (debtCredit.DueDate <= DateTime.Now.AddDays(7))
                {
                    return View("UpcomingPaymentNotification", debtCredit);
                }
            }

            return RedirectToAction("Index");
        }

        // Логика для расчета графика погашения кредита с аннуитетными платежами
        private List<PaymentScheduleModel> CalculateLoanSchedule(decimal loanAmount, decimal interestRate, DateTime dueDate)
        {
            // Пример расчета аннуитетного платежа
            decimal monthlyInterestRate = interestRate / 100 / 12;
            int totalMonths = (dueDate.Year - DateTime.Now.Year) * 12 + dueDate.Month - DateTime.Now.Month;
            decimal monthlyPayment = loanAmount * (monthlyInterestRate * (decimal)Math.Pow(1 + (double)monthlyInterestRate, totalMonths)) /
                                    ((decimal)Math.Pow(1 + (double)monthlyInterestRate, totalMonths) - 1);

            var schedule = new List<PaymentScheduleModel>();
            var paymentDate = DateTime.Now;

            for (int i = 1; i <= totalMonths; i++) // Используем общее количество месяцев до даты погашения
            {
                paymentDate = paymentDate.AddMonths(1);
                schedule.Add(new PaymentScheduleModel
                {
                    PaymentDate = paymentDate,
                    PaymentAmount = monthlyPayment,
                    RemainingAmount = loanAmount - (monthlyPayment * i)
                });
            }

            return schedule;
        }

        public class PaymentScheduleModel
        {
            public DateTime PaymentDate { get; set; }
            public decimal PaymentAmount { get; set; }
            public decimal RemainingAmount { get; set; }
        }
    }
}
