using FinalYearProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class LeaveApproveController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public LeaveApproveController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
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
                            return View(await _db.Leave.ToListAsync());
                        }
                        else
                        {
                            var leaveList = await (from leave in _db.Leave
                                             join emp in _db.EmployeeDetails on leave.staff_id equals emp.employee_id
                                             join com in _db.Company on emp.parent_company equals com.company_id
                                             where com.current_admin == currentAdmin.admin_id
                                             select leave).ToListAsync();

                            return View(leaveList);
                        }
                    }

                }
                else
                {
                    var leaveList = await (from leave in _db.Leave
                                           join emp in _db.EmployeeDetails on leave.staff_id equals emp.employee_id
                                           where emp.employer_id == currentUser.employee_id
                                           select leave).ToListAsync();

                    return View(leaveList);
                }
            }
        }

        public async Task<IActionResult> Decide(string id)
        {
            List<string> status = new List<string>();
            status.Add("Approved");
            status.Add("Rejected");

            ViewBag.status = new SelectList(status.AsEnumerable());

            var leave = await _db.Leave.FindAsync(id);

            if (leave.approval_status != "")
            {
                

                if (leave == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(leave);
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decide(Leave leave)
        {
            var aspId = User.Identity?.Name;
            string sessionId;

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
                        sessionId = currentAdmin.admin_id;
                    }
                }
                else
                {
                    sessionId = currentUser.employee_id;
                }
            }

           
            if (ModelState.IsValid)
            {
                leave.approved_by = sessionId;

                var duration = leave.leave_end - leave.leave_start;
                var hoursUsed = (int)Math.Ceiling(duration.TotalHours);

                var employeeAffected = await _db.EmployeeDetails.FindAsync(leave.staff_id);

                if (employeeAffected == null)
                {
                    return NotFound();
                }

                if (leave.approval_status == "Approved")
                {
                    if (leave.leaveType == "Paid")
                    {
                        employeeAffected.paidLeaveOnBargain -= hoursUsed;
                    }
                    else if (leave.leaveType == "Sick")
                    {
                        employeeAffected.sickLeaveOnBargain -= hoursUsed;
                    }

                }
                else
                {
                    if (leave.leaveType == "Paid")
                    {
                        employeeAffected.paidLeaveHourLeft += hoursUsed;
                        employeeAffected.paidLeaveOnBargain -= hoursUsed;
                    }
                    else if (leave.leaveType == "Sick")
                    {
                        employeeAffected.sickLeaveHourLeft += hoursUsed;
                        employeeAffected.sickLeaveOnBargain -= hoursUsed;
                    }
                }

                _db.Update(employeeAffected);
                _db.Update(leave);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(leave);
        }

        public async Task<FileResult> Download(string? id)
        {
            var leave = await _db.Leave.FindAsync(id);

            string path = _hostingEnvironment.WebRootPath + leave.doc_filepath;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", leave.staff_id + "_" + leave.leave_id + Path.GetExtension(path));
        }
    }
}
