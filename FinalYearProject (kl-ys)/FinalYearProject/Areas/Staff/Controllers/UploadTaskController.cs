using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UploadTaskController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UploadTaskController(IEmailSender emailSender, UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
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
                    var currentAdmin = await _db.Admin.Where(a => a.admin_id == aspId).FirstOrDefaultAsync();

                    if (currentAdmin == null)
                    {
                        return RedirectToAction("Login", "Account", new { area = "Identity" });
                    }
                    else
                    {
                        if (currentAdmin.is_superadmin)
                        {
                            var viewModel = new EmployeeTaskListVM
                            {
                                TaskCreated = await _db.EmployeeTasks.ToListAsync(),
                            };
                            return View(viewModel);
                        }
                        else
                        {
                            var employeetaskList = await _db.EmployeeTasks.Where(c => c.current_admin == currentAdmin.admin_id).ToListAsync();
                            EmployeeTaskListVM list = new EmployeeTaskListVM()
                            {
                                owner = currentAdmin,
                                TaskCreated = employeetaskList,
                                displayEmployeeTask = new Models.EmployeeTask(),
                            };
                            return View(list);
                        }
                    }
                }
                else
                {
                    return View();
                }
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.staff_id = new SelectList(await _db.EmployeeDetails.ToListAsync(), "employee_id", "employee_name");

            var newTask = new EmployeeTask
            {
                date_upload = DateTime.Now,
                emtask_id = await GenerateTaskID(),
            };
            return View(newTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeTask task, IFormFile emtaskDetail)
        {
            if (ModelState.IsValid)
            {
                var aspId = User.Identity?.Name;
                var currentAdmin = await _db.Admin.Where(a => a.admin_id == aspId).FirstOrDefaultAsync();
                task.current_admin = currentAdmin.admin_id;

                if (emtaskDetail != null)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var uploads = Path.Combine(webRootPath, "tasks");
                    var extension = Path.GetExtension(emtaskDetail.FileName);
                    var filePath = task.emtask_id + extension;

                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    var filePathToSave = Path.Combine(uploads, filePath);

                    using (var fileStream = new FileStream(filePathToSave, FileMode.Create))
                    {
                        emtaskDetail.CopyTo(fileStream);
                    }

                    task.emtaskDetail = @"\tasks\" + filePath;
                }

                _db.EmployeeTasks.Add(task);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            ViewBag.staff_id = new SelectList(await _db.EmployeeDetails.ToListAsync(), "employee_id", "employee_name", task.staff_id);
            return View(task);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var task = await _db.EmployeeTasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            EmployeeEditTaskVM editVM = new EmployeeEditTaskVM
            {
                employeetask = task,
                emtask_duration = (int)task.emtask_duration,
            };

            return View(editVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeEditTaskVM editTask, IFormFile? emtaskDetail = null)
        {
            var existingTask = await _db.EmployeeTasks.FindAsync(editTask.employeetask.emtask_id);

            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.emtask_duration = editTask.emtask_duration;

            if (emtaskDetail != null && emtaskDetail.Length > 0)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var uploads = Path.Combine(webRootPath, "employeetasks");

                Directory.CreateDirectory(uploads);

                var extension = Path.GetExtension(emtaskDetail.FileName);
                var filename = existingTask.emtask_id + extension;
                var filepath = Path.Combine(uploads, filename);

                using (var filestream = new FileStream(filepath, FileMode.Create))
                {
                    emtaskDetail.CopyTo(filestream);
                }

                existingTask.emtaskDetail = @"\employeetasks\" + filename;
            }

            _db.Update(existingTask);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var task = await _db.EmployeeTasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            _db.EmployeeTasks.Remove(task);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> View(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeetask = await _db.EmployeeTasks.FindAsync(id);

            if (employeetask == null)
            {
                return NotFound();
            }

            return View(employeetask);
        }

        public async Task<FileResult> Download(string? id)
        {
            var employeetasks = await _db.EmployeeTasks.FindAsync(id);

            string path = _hostingEnvironment.WebRootPath + employeetasks.emtaskdoneFile;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", employeetasks.staff_id + "_" + employeetasks.emtask_id + Path.GetExtension(path));
        }

        private async Task<string> GenerateTaskID()
        {
            string newId;
            string prefix = "P";

            var totalTasks = await _db.EmployeeTasks.CountAsync();

            newId = prefix + (totalTasks + 1).ToString("00000");

            return newId;
        }
    }
}
