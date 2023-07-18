using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class LeaveApplyController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public LeaveApplyController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
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
                    var leaveList = await _db.Leave.Where(l => l.staff_id == currentUser.employee_id).ToListAsync();

                    EmployeeLeaveListVM list = new EmployeeLeaveListVM()
                    {
                        owner = currentUser,
                        leaveApplied = leaveList,
                        displayLeave = new Models.Leave(),
                    };

                    return View(list);
                }
            }

        }

        public async Task<IActionResult> Create(string id)
        {
            List<string> leaveType = new List<string>
            {
                "Paid",
                "Sick",
                "Unpaid"
            };

            ViewBag.leaveType = new SelectList(leaveType.AsEnumerable());

            var employeeSelected = await _db.EmployeeDetails.FindAsync(id);

            if (employeeSelected == null)
            {
                return NotFound();
            }
            else
            {
                Models.Leave leave = new Models.Leave()
                {
                    staff_id = employeeSelected.employee_id,
                    leave_start = DateTime.Now,
                    leave_end = DateTime.Now.AddDays(1),
                    leave_id = GenerateLeaveID(employeeSelected.employee_id).Result
                };

                return View(leave);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leave leave)
        {
            var employeeSelected = await _db.EmployeeDetails.FindAsync(leave.staff_id);

            if (employeeSelected == null)
            {
                return NotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (leave.leave_end < leave.leave_start)
                    {
                        ViewBag.message = "Invalid duration.";
                        return View(leave);
                    }
                    else if ((leave.leave_end - leave.leave_start).TotalHours < 1)
                    {
                        ViewBag.message = "Duration of leave must be more than one hour.";
                        return View(leave);
                    }

                    var appliedLeave = await _db.Leave.Where(l => l.staff_id == employeeSelected.employee_id).ToListAsync();

                    foreach (Leave l in appliedLeave)
                    {
                        if (l.leave_start < leave.leave_end && l.leave_end >= leave.leave_start)
                        {
                            ViewBag.message = "Overlaps with existing leave application. You have applied a leave for " + l.leave_start + " to " + l.leave_end;
                            return View(leave);
                        }
                    }

                    var duration = leave.leave_end - leave.leave_start;

                    var hoursUsed = (int)Math.Ceiling(duration.TotalHours);

                    if (leave.leaveType == "Sick")
                    {
                        if (employeeSelected.sickLeaveHourLeft >= hoursUsed)
                        {
                            employeeSelected.sickLeaveHourLeft -= hoursUsed;
                            employeeSelected.sickLeaveOnBargain += hoursUsed;

                            _db.Update(employeeSelected);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            ViewBag.message = "You do not have enough sick leaves left.";
                            return View(leave);
                        }
                    }
                    else if (leave.leaveType == "Paid")
                    {
                        if (employeeSelected.paidLeaveHourLeft >= hoursUsed)
                        {
                            employeeSelected.paidLeaveHourLeft -= hoursUsed;
                            employeeSelected.paidLeaveOnBargain += hoursUsed;

                           
                        }
                        else
                        {
                            ViewBag.message = "You do not have enough paid leaves left.";
                            return View(leave);
                        }
                    }

                    _db.Update(employeeSelected);
                    await _db.SaveChangesAsync();

                    _db.Leave.Add(leave);
                    await _db.SaveChangesAsync();

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var leaveFromDb = await _db.Leave.FindAsync(leave.leave_id);

                    if (files.Count > 0)
                    {
                        var uploads = Path.Combine(webRootPath, "leaves");
                        var extension = Path.GetExtension(files[0].FileName);
                        var filepath = leaveFromDb.leave_id + extension;

                        using (var filestream = new FileStream(Path.Combine(uploads, filepath), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }

                        leaveFromDb.doc_filepath = @"\leaves\" + filepath;

                        await _db.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }

                return View(leave);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var leave = await _db.Leave.FindAsync(id);

            List<string> leaveType = new List<string>
            {
                "Paid",
                "Sick",
                "Unpaid"
            };

            ViewBag.leaveType = new SelectList(leaveType.AsEnumerable());

            if (leave == null)
            {
                return NotFound();
            }
            else
            {
                if (leave.approval_status == null)
                {
                    EmployeeEditLeaveVM editVM = new EmployeeEditLeaveVM()
                    {
                        leave = leave,
                        previousType = leave.leaveType,
                        previousDuration = (int)Math.Ceiling((leave.leave_end - leave.leave_start).TotalHours)
                    };

                    return View(editVM);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }

            }



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeEditLeaveVM editLeave)
        {
            var employeeSelected = await _db.EmployeeDetails.FindAsync(editLeave.leave.staff_id);

            if (employeeSelected == null)
            {
                return NotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (editLeave.leave.leave_end < editLeave.leave.leave_start)
                    {
                        ViewBag.message = "Invalid duration.";
                        return View(editLeave);
                    }
                    else if ((editLeave.leave.leave_end - editLeave.leave.leave_start).TotalHours < 1)
                    {
                        ViewBag.message = "Duration of leave must be more than one hour.";
                        return View(editLeave);
                    }

                    var appliedLeave = await _db.Leave.Where(l => l.staff_id == employeeSelected.employee_id && l.leave_id != editLeave.leave.leave_id).ToListAsync();

                    foreach (Leave l in appliedLeave)
                    {
                        if (l.leave_start < editLeave.leave.leave_end && l.leave_end >= editLeave.leave.leave_start)
                        {
                            ViewBag.message = "Overlaps with existing leave application. You have applied a leave for " + l.leave_start + " to " + l.leave_end;
                            return View(editLeave);
                        }
                    }

                    //Check Time Logic
                    //Updated Used Time

                    var duration = editLeave.leave.leave_end - editLeave.leave.leave_start;
                    var hoursUsed = (int)Math.Ceiling(duration.TotalHours);

                    if (editLeave.previousType == "Paid")
                    {
                        employeeSelected.paidLeaveOnBargain -= editLeave.previousDuration;
                        employeeSelected.paidLeaveHourLeft += editLeave.previousDuration;
                    }
                    else if (editLeave.previousType == "Sick")
                    {
                        employeeSelected.sickLeaveOnBargain -= editLeave.previousDuration;
                        employeeSelected.sickLeaveHourLeft += editLeave.previousDuration;
                    }


                    if (editLeave.leave.leaveType == "Sick")
                    {
                        if (employeeSelected.sickLeaveHourLeft >= hoursUsed)
                        {
                            employeeSelected.sickLeaveHourLeft -= hoursUsed;
                            employeeSelected.sickLeaveOnBargain += hoursUsed;

                        }
                        else
                        {
                            ViewBag.message = "You do not have enough sick leaves left.";
                            return View(editLeave);
                        }
                    }
                    else if (editLeave.leave.leaveType == "Paid")
                    {
                        if (employeeSelected.paidLeaveHourLeft >= hoursUsed)
                        {
                            employeeSelected.paidLeaveHourLeft -= hoursUsed;
                            employeeSelected.paidLeaveOnBargain += hoursUsed;

                        }
                        else
                        {
                            ViewBag.message = "You do not have enough paid leaves left.";
                            return View(editLeave);
                        }
                    }

                    _db.Update(employeeSelected);
                    await _db.SaveChangesAsync();
                    //---------

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;

                    if (files.Count > 0)
                    {
                        var uploads = Path.Combine(webRootPath, "leaves");
                        var extension = Path.GetExtension(files[0].FileName);
                        var filepath = editLeave.leave.leave_id + extension;

                        using (var filestream = new FileStream(Path.Combine(uploads, filepath), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }

                        editLeave.leave.doc_filepath = @"\leaves\" + filepath;

                    }

                    _db.Update(editLeave.leave);
                    await _db.SaveChangesAsync();



                    return RedirectToAction(nameof(Index));
                }
            }

            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var leave = await _db.Leave.FindAsync(id);

            if (leave == null)
            {
                return NotFound();
            }
            else
            {
                if (leave.approval_status != null)
                {
                    return View(leave);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }

            }

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            var leave = await _db.Leave.FindAsync(id);

            if (leave == null)
            {
                return View();
            }

            _db.Leave.Remove(leave);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<string> GenerateLeaveID(string empId)
        {
            string newId;
            string prefix = empId + "-L";

            var totalLeave = await _db.Leave.CountAsync();

            newId = prefix + (totalLeave + 1).ToString("00000");

            return newId;
        }

        
    }
}
