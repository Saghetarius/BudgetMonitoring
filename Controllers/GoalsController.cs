using BudgetMonitoring.Data;
using BudgetMonitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BudgetMonitoring.Controllers
{
    public class GoalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GoalsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Страница с активными и завершенными целями
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Получаем активные цели пользователя
            var activeGoals = await _context.Goals
                .Where(g => g.UserId == user.UserName && !g.IsCompleted)
                .ToListAsync();

            // Получаем завершенные цели пользователя
            var completedGoals = await _context.Goals
                .Where(g => g.UserId == user.UserName && g.IsCompleted)
                .ToListAsync();

            // Передаем данные в представление
            var viewModel = new GoalViewModel
            {
                ActiveGoals = activeGoals,
                CompletedGoals = completedGoals
            };

            return View(viewModel);
        }

        // Метод для завершения цели
        [HttpPost]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return NotFound(); 
            }
            goal.IsCompleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }





        // Страница для добавления новой цели
        public IActionResult Create()
        {
            return View();
        }

        // Метод для сохранения новой цели
        [HttpPost]
        public async Task<IActionResult> Create(Goal goal)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                goal.UserId = user.UserName;  
                goal.IsCompleted = false;     
                goal.UserId = user.UserName;  
                goal.IsCompleted = false;     

                _context.Goals.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(goal);
        }
    }
}
