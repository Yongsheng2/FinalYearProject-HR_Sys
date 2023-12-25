using FinalYearProject.Data;
using FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class HandleTaskController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HandleTaskController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index(string id)
        {
            var aspId = User.Identity?.Name;

            if (aspId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            else
            {
                var currentUser = await _db.EmployeeDetails.Where(e => e.user_id == aspId).FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    var taskList = await _db.EmployeeTasks.Where(c => c.staff_id == currentUser.employee_id).ToListAsync();

                    EmployeeTaskListVM list = new EmployeeTaskListVM()
                    {
                        owners = currentUser,
                        TaskCreated = taskList,
                        displayEmployeeTask = new Models.EmployeeTask(),
                    };

                    return View(list);
                }
            }
        }

        public async Task<IActionResult> Submit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _db.EmployeeTasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string emtask_id, IFormFile emtaskdoneFile)
        {
            if (emtask_id == null)
            {
                return NotFound();
            }

            var task = await _db.EmployeeTasks.FindAsync(emtask_id);

            if (task == null)
            {
                return NotFound();
            }

            if (emtaskdoneFile != null)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var uploads = Path.Combine(webRootPath, "employeedonetasks");
                var extension = Path.GetExtension(emtaskdoneFile.FileName);
                var filePath = task.emtask_id + extension;

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var filePathToSave = Path.Combine(uploads, filePath);

                using (var fileStream = new FileStream(filePathToSave, FileMode.Create))
                {
                    emtaskdoneFile.CopyTo(fileStream);
                }

                task.emtaskdoneFile = @"\employeedonetasks\" + filePath;
            }

            task.progress_status = "Complete";

            _db.EmployeeTasks.Update(task);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Download(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeetasks = await _db.EmployeeTasks.FindAsync(id);

            if (employeetasks == null || employeetasks.emtaskDetail == null)
            {
                return NotFound();
            }

            string path = Path.Combine(_hostingEnvironment.WebRootPath, employeetasks.emtaskDetail.TrimStart('\\', '/'));

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", employeetasks.staff_id + "_" + employeetasks.emtask_id + Path.GetExtension(path));
        }
    }
}

