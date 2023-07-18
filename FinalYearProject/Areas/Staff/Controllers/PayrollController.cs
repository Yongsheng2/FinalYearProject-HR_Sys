using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class PayrollController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PayrollController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Payroll.ToListAsync());
        }
        public async Task<IActionResult> ViewPayrate()
        {
            return View(await _db.Payrate.ToListAsync());
        }
        public async Task<IActionResult> CreatePayroll()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayroll(Models.Payroll payroll )
        {
            var payrollDate = from s in _db.Payroll where s.staff_id == payroll.staff_id select s.date_created;
            DateTime[] payrollDateArray = payrollDate.ToArray();

            var shift = from s in _db.Attendance where s.staff_id == payroll.staff_id select s.shift_id;
            string[] shiftID = shift.ToArray();
            var overTimeSalary = 0.00f;
            var normalSalary = 0.00f;
            if(payrollDateArray.Length == 0)
            {
                Array.Resize(ref payrollDateArray, 1);
            }
            for (int i = 0; i < shiftID.Length; i++)
            {
                var startTime = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.shift_start).FirstOrDefaultAsync();
                if (payrollDateArray[payrollDateArray.Length - 1] < startTime && startTime < DateTime.Now)
                {
                    var isOverTime = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.is_overtime).FirstOrDefaultAsync();
                    if (isOverTime == true)
                    {

                        var attendaceEndTime = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.shift_end).FirstOrDefaultAsync();
                        var shiftEndTime = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.shift_end).FirstOrDefaultAsync();
                        var otID = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.payrate_id).FirstOrDefaultAsync();
                        var otpayrate = await (from s in _db.Payrate where otID == s.payrate_id select s.payrate_ratePerHour).FirstOrDefaultAsync();
                        if (attendaceEndTime > shiftEndTime)
                        {
                            TimeSpan otTime = (TimeSpan)(shiftEndTime - startTime);
                            overTimeSalary += (float)otTime.Minutes * (float)(otpayrate / 60.00f);
                        }
                        else
                        {
                            TimeSpan otTime = (TimeSpan)(attendaceEndTime - startTime);
                            overTimeSalary += (float)otTime.Minutes * (float)(otpayrate / 60.00f);
                        }
                    }
                    else
                    {
                        var endTime = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.shift_end).FirstOrDefaultAsync();
                        var payrateid = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.payrate_id).FirstOrDefaultAsync();
                        var payrate = await (from s in _db.Payrate where payrateid == s.payrate_id select s.payrate_ratePerHour).FirstOrDefaultAsync();
                        TimeSpan timeDiff = (TimeSpan)(endTime - startTime);
                        normalSalary += (float)timeDiff.TotalHours * (float)payrate;
                    }
                }

            }
            var totalSalary = normalSalary + overTimeSalary;
            float kwsp = totalSalary * 0.11f;
            payroll.kwsp_total = kwsp;
            var netSalary = totalSalary - kwsp;
            if (totalSalary <= 1000.00f)
            {
                if (0.00f < totalSalary && totalSalary <= 1000.00f)
                {
                    if (0.00f < totalSalary && totalSalary <= 100.00f)
                    {
                        if(0.00f < totalSalary && totalSalary <= 30.00f)
                        {
                            netSalary -= 0.10f;
                        }
                        else if(30.00f < totalSalary && totalSalary <= 50.00f)
                        {
                            netSalary -= 0.20f;
                        }
                        else if (50.00f < totalSalary && totalSalary <= 70.00f)
                        {
                            netSalary -= 0.30f;
                        }
                        else
                        {
                            netSalary -= 0.40f;
                        }
                    }
                    else if (100.00f < totalSalary && totalSalary <= 140.00f)
                    {
                        netSalary -= 0.60f;
                    }
                    else if (140.00f < totalSalary && totalSalary <= 200.00f)
                    {
                        netSalary -= 0.85f;
                    }
                    else if (200.00f < totalSalary && totalSalary <= 300.00f)
                    {
                        netSalary -= 1.25f;
                    }
                    else if (300.00f < totalSalary && totalSalary <= 400.00f)
                    {
                        netSalary -= 1.75f;
                    }
                    else
                    {
                        netSalary -= 2.25f;
                    }
                }
                else if (500.00f < totalSalary && totalSalary <= 1000.00f)
                {
                    if (500.00f < totalSalary && totalSalary <= 600.00f)
                    {
                        netSalary -= 2.75f;
                    }
                    else if (600.00f < totalSalary && totalSalary <= 700.00f)
                    {
                        netSalary -= 3.25f;
                    }
                    else if (700.00f < totalSalary && totalSalary <= 800.00f)
                    {
                        netSalary -= 3.75f;
                    }
                    else if (800.00f < totalSalary && totalSalary <= 900.00f)
                    {
                        netSalary -= 4.25f;
                    }
                    else
                    {
                        netSalary -= 4.75f;
                    }
                }
            }
            else if(1000.00f < totalSalary && totalSalary<= 2000.00f)
            {
                if(1000.00f < totalSalary && totalSalary <= 1500.00f)
                {
                    if(1000.00f < totalSalary && totalSalary <= 1100.00f)
                    {
                        netSalary -= 5.25f;
                    }
                    else if(1100.00f < totalSalary && totalSalary <= 1200.00f)
                    {
                        netSalary -= 5.75f;
                    }
                    else if (1200.00f < totalSalary && totalSalary <= 1300.00f)
                    {
                        netSalary -= 6.25f;
                    }
                    else if (1300.00f < totalSalary && totalSalary <= 1400.00f)
                    {
                        netSalary -= 6.75f;
                    }
                    else
                    {
                        netSalary -= 7.250f;
                    }
                }
                else if (1500.00f < totalSalary && totalSalary <= 2000.00f)
                {
                    if (1500.00f < totalSalary && totalSalary <= 1600.00f)
                    {
                        netSalary -= 7.75f;
                    }
                    else if (1600.00f < totalSalary && totalSalary <= 1700.00f)
                    {
                        netSalary -= 8.25f;
                    }
                    else if (1700.00f < totalSalary && totalSalary <= 1800.00f)
                    {
                        netSalary -= 8.75f;
                    }
                    else if (1800.00f < totalSalary && totalSalary <= 1900.00f)
                    {
                        netSalary -= 9.25f;
                    }
                    else
                    {
                        netSalary -= 9.75f;
                    }
                }
            }
            else if (2000.00f < totalSalary && totalSalary <= 3000.00f)
            {
                if (2000.00f < totalSalary && totalSalary <= 2500.00f)
                {
                    if (2000.00f < totalSalary && totalSalary <= 2100.00f)
                    {
                        netSalary -= 10.25f;
                    }
                    else if (2100.00f < totalSalary && totalSalary <= 2200.00f)
                    {
                        netSalary -= 10.75f;
                    }
                    else if (2200.00f < totalSalary && totalSalary <= 2300.00f)
                    {
                        netSalary -= 11.25f;
                    }
                    else if (2300.00f < totalSalary && totalSalary <= 2400.00f)
                    {
                        netSalary -= 11.75f;
                    }
                    else
                    {
                        netSalary -= 12.25f;
                    }
                }
                else if (2500.00f < totalSalary && totalSalary <= 3000.00f)
                {
                    if (2500.00f < totalSalary && totalSalary <= 2600.00f)
                    {
                        netSalary -= 12.75f;
                    }
                    else if (2600.00f < totalSalary && totalSalary <= 2700.00f)
                    {
                        netSalary -= 13.25f;
                    }
                    else if (2700.00f < totalSalary && totalSalary <= 2800.00f)
                    {
                        netSalary -= 13.75f;
                    }
                    else if (2800.00f < totalSalary && totalSalary <= 2900.00f)
                    {
                        netSalary -= 14.25f;
                    }
                    else
                    {
                        netSalary -= 14.75f;
                    }
                }
            }
            else if (3000.00f < totalSalary && totalSalary <= 4000.00f)
            {
                if (3000.00f < totalSalary && totalSalary <= 3500.00f)
                {
                    if (3000.00f < totalSalary && totalSalary <= 3100.00f)
                    {
                        netSalary -= 15.25f;
                    }
                    else if (3100.00f < totalSalary && totalSalary <= 3200.00f)
                    {
                        netSalary -= 15.75f;
                    }
                    else if (3200.00f < totalSalary && totalSalary <= 3300.00f)
                    {
                        netSalary -= 16.25f;
                    }
                    else if (3400.00f < totalSalary && totalSalary <= 3400.00f)
                    {
                        netSalary -= 16.75f;
                    }
                    else
                    {
                        netSalary -= 17.25f;
                    }
                }
                else if (3500.00f < totalSalary && totalSalary <= 4000.00f)
                {
                    if (3500.00f < totalSalary && totalSalary <= 3600.00f)
                    {
                        netSalary -= 17.75f;
                    }
                    else if (3600.00f < totalSalary && totalSalary <= 3700.00f)
                    {
                        netSalary -= 18.25f;
                    }
                    else if (3700.00f < totalSalary && totalSalary <= 3800.00f)
                    {
                        netSalary -= 18.75f;
                    }
                    else if (3800.00f < totalSalary && totalSalary <= 3900.00f)
                    {
                        netSalary -= 19.25f;
                    }
                    else
                    {
                        netSalary -= 19.75f;
                    }
                }
            }
            else if (4000.00f < totalSalary && totalSalary <= 5000.00f)
            {
                if (4000.00f < totalSalary && totalSalary <= 4500.00f)
                {
                    if (4000.00f < totalSalary && totalSalary <= 4100.00f)
                    {
                        netSalary -= 20.25f;
                    }
                    else if (4100.00f < totalSalary && totalSalary <= 4200.00f)
                    {
                        netSalary -= 20.75f;
                    }
                    else if (4200.00f < totalSalary && totalSalary <= 4300.00f)
                    {
                        netSalary -= 21.25f;
                    }
                    else if (4300.00f < totalSalary && totalSalary <= 4400.00f)
                    {
                        netSalary -= 21.75f;
                    }
                    else
                    {
                        netSalary -= 22.25f;
                    }
                }
                else if (4500.00f < totalSalary && totalSalary <= 5000.00f)
                {
                    if (4500.00f < totalSalary && totalSalary <= 4600.00f)
                    {
                        netSalary -= 22.75f;
                    }
                    else if (4600.00f < totalSalary && totalSalary <= 4700.00f)
                    {
                        netSalary -= 23.25f;
                    }
                    else if (4700.00f < totalSalary && totalSalary <= 4800.00f)
                    {
                        netSalary -= 23.75f;
                    }
                    else if (4800.00f < totalSalary && totalSalary <= 4900.00f)
                    {
                        netSalary -= 24.25f;
                    }
                    else
                    {
                        netSalary -= 24.75f;
                    }
                }
            }
            else
            {
                netSalary -= 24.75f;
            }
            var zakat = totalSalary * 0.025f;
            payroll.zakat_total = zakat;
            netSalary -= zakat;
            if (0 <= totalSalary && totalSalary <= 5000.00f)
            {

            }
            else if (5000.00f < totalSalary && totalSalary <= 20000.00f)
            {
                netSalary = totalSalary + ((totalSalary - 5000.00f) * 0.01f);
            }
            else if (20000.00f < totalSalary && totalSalary <= 35000.00f)
            {
                netSalary = totalSalary + ((totalSalary - 20000.00f) * 0.03f);

            }
            else if (35000.00f < totalSalary && totalSalary <= 50000.00f)
            {
                netSalary = totalSalary + ((totalSalary - 35000.00f) * 0.08f);
            }
            payroll.month_salary = netSalary;
            var staffID = from s in _db.Payroll where s.staff_id == payroll.staff_id select s.payroll_id;
            string[] staff = staffID.ToArray(); 
            payroll.payroll_id = "PR" + payroll.staff_id.ToString() + staff.Length;
            _db.Payroll.Add(payroll);
            await _db.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SD.PayrateManage)]
        public async Task<IActionResult> CreatePayrate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayrate(Payrate payrate)
        {
            payrate.payrate_id = "PR" + (int)payrate.payrate_ratePerHour;
            var checking = await (from s in _db.Payrate where s.payrate_id == payrate.payrate_id select s.payrate_ratePerHour).FirstOrDefaultAsync();
            if(checking == null)
            {
                if (ModelState.IsValid)
                {
                    _db.Payrate.Add(payrate);
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(ViewPayrate));
                }
            }
            ModelState.AddModelError("Custom Error", "The number of Payrate per hour has exist with unique ID");

            return View(payrate);
        }

        [Authorize(Roles = SD.PayrateManage)]
        public async Task<IActionResult> EditPayrate(string? id)
        {
            var payrate = await _db.Payrate.FindAsync(id);
            if(payrate == null)
            {
                return NotFound();
            }
            return View(payrate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPayrate(Payrate payrate)
        {
            var checking = await (from s in _db.Payrate where s.payrate_id == payrate.payrate_id select s.payrate_ratePerHour).FirstOrDefaultAsync();
            if (checking == null)
            {
                payrate.payrate_id = "PR" + (int)payrate.payrate_ratePerHour;
                if (ModelState.IsValid)
                {
                    _db.Payrate.Add(payrate);
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(ViewPayrate));
                }
            }
            ModelState.AddModelError("Custom Error", "The number of Payrate per hour has exist with unique ID");

            return View(payrate);
        }

        public async Task<IActionResult> ViewPayroll()
        {
            var aspId = User.Identity?.Name;

            if (aspId == null)
            {
                return RedirectToAction(default);
            }
            else
            {
                var currentUser = await(from s in _db.EmployeeDetails where s.employee_id == aspId select s.employee_id).FirstOrDefaultAsync();

                return View(await _db.Payroll.Where(e => e.staff_id == currentUser).ToListAsync());

            }
        }

    }
}
