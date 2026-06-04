using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetaForest.Data;
using MetaForest.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MetaForest.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tasks List
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User) ?? string.Empty;

                var tasks = await _context.TaskItems
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                // İstatistikler
                ViewBag.TotalTasks = tasks.Count;
                ViewBag.CompletedTasks = tasks.Count(t => t.IsCompleted);
                ViewBag.PendingTasks = tasks.Count(t => !t.IsCompleted);

                if (ViewBag.TotalTasks > 0)
                {
                    ViewBag.CompletionPercentage = (ViewBag.CompletedTasks * 100) / ViewBag.TotalTasks;
                }
                else
                {
                    ViewBag.CompletionPercentage = 0;
                }

                return View(tasks);
            }
            catch (Exception ex)
            {
                // Veritabanı tablosu oluşturulmamışsa, boş liste dön
                ViewBag.TotalTasks = 0;
                ViewBag.CompletedTasks = 0;
                ViewBag.PendingTasks = 0;
                ViewBag.CompletionPercentage = 0;
                ViewBag.Error = $"Görevler yüklenirken hata oluştu: {ex.Message}";
                return View(new List<TaskItem>());
            }
        }

        // POST: Add Task
        [HttpPost]
        public async Task<IActionResult> Add(TaskItem taskItem)
        {
            try
            {
                var userId = _userManager.GetUserId(User) ?? string.Empty;

                if (string.IsNullOrWhiteSpace(taskItem.Title))
                {
                    return RedirectToAction("Index");
                }

                var newTask = new TaskItem
                {
                    UserId = userId,
                    Title = taskItem.Title.Trim(),
                    Description = taskItem.Description?.Trim() ?? string.Empty,
                    Priority = taskItem.Priority ?? "Medium",
                    IsCompleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.TaskItems.Add(newTask);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Görev eklenirken bir hata oluştu.");
                return RedirectToAction("Index");
            }
        }

        // POST: Toggle Task Completion
        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User) ?? string.Empty;
                var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                if (task == null)
                {
                    return NotFound();
                }

                task.IsCompleted = !task.IsCompleted;
                task.UpdatedAt = DateTime.Now;
                _context.TaskItems.Update(task);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Delete Task
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User) ?? string.Empty;
                var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                if (task == null)
                {
                    return NotFound();
                }

                _context.TaskItems.Remove(task);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}
