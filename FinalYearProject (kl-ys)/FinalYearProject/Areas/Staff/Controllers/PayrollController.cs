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
            var errorMessage = TempData["ErrorMessage"] as string;

            // Pass the error message to the view
            ViewData["ErrorMessage"] = errorMessage;

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
        public async Task<IActionResult> CreatePayroll(Models.Payroll payroll,DateTime start_date, DateTime end_date )
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
                var startTime = await (from s in _db.Shift where s.shift_id == shiftID[i]  && s.shift_start >= start_date && s.shift_start <= end_date select s.shift_start).FirstOrDefaultAsync();
                var endTime = await (from s in _db.Shift where s.shift_id == shiftID[i] && s.shift_end <= end_date select s.shift_end).FirstOrDefaultAsync();

                var shiftToClaim = await 
                    (from s in _db.Shift where s.shift_id == shiftID[i] 
                     && s.shift_start >= start_date && s.shift_start <= end_date 
                     && s.shift_end <= end_date select s.shift_id).FirstOrDefaultAsync();

                var attendanceClaim = await _db.Attendance.Where(ac => ac.shift_id == shiftToClaim && ac.claimed == false
                                          && ac.start_time >= start_date && ac.start_time <= end_date
                                      && ac.end_time <= end_date).FirstOrDefaultAsync();

                if (attendanceClaim != null)
                {
                    var isOverTime = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.is_overtime).FirstOrDefaultAsync();
                    if (isOverTime == true)
                    {

                        var attendaceEndTime = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.shift_end).FirstOrDefaultAsync();
                        var shiftEndTime = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.shift_end).FirstOrDefaultAsync();
                        var otID = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.payrate_id).FirstOrDefaultAsync();
                        var otpayrate = await (from s in _db.Payrate where otID == s.payrate_id select s.payrate_ratePerHour).FirstOrDefaultAsync();
                        if (attendaceEndTime > shiftEndTime && attendaceEndTime <= end_date && startTime <= start_date)
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
                        var payrateid = await (from s in _db.Shift where s.shift_id == shiftID[i] select s.payrate_id).FirstOrDefaultAsync();
                        var payrate = await (from s in _db.Payrate where payrateid == s.payrate_id select s.payrate_ratePerHour).FirstOrDefaultAsync();
                        TimeSpan timeDiff = (TimeSpan)(endTime - startTime);
                        normalSalary += (float)timeDiff.TotalHours * (float)payrate;
                    }
                }

            }

            if (normalSalary == 0 && overTimeSalary==0) {
                TempData["ErrorMessage"] = "No attendance to claim";
                return RedirectToAction(nameof(Index));
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
            payroll.payroll_id = GeneratePayRollID().Result;

            //kl part for incentives
            var currentStaffCompany = await _db.EmployeeDetails.Where(ed => ed.employee_id == payroll.staff_id).Select(ed => ed.parent_company).FirstOrDefaultAsync();
            var incentivesID = await _db.Incentives.Where(i => i.company_id == currentStaffCompany).Select(i=>i.incentives_id).FirstOrDefaultAsync();
            string monthString;
            float incentive_total = 0;
            switch (DateTime.Now.Month)
            {
                case 1:
                    monthString = "Jan";
                    break;
                case 2:
                    monthString = "Feb";
                    break;
                case 3:
                    monthString = "Mar";
                    break;
                case 4:
                    monthString = "Apr";
                    break;
                case 5:
                    monthString = "May";
                    break;
                case 6:
                    monthString = "Jun";
                    break;
                case 7:
                    monthString = "Jul";
                    break;
                case 8:
                    monthString = "Aug";
                    break;
                case 9:
                    monthString = "Sep";
                    break;
                case 10:
                    monthString = "Oct";
                    break;
                case 11:
                    monthString = "Nov";
                    break;
                case 12:
                    monthString = "Dec";
                    break;
                default:
                    monthString = "Invalid Month";
                    break;
            }

              
            // Check if any leave_id is not null
            var leaveCheck = from s in _db.Attendance
                                where s.staff_id == payroll.staff_id && s.leave_id != null
                                && (s.start_time >= start_date
                                || s.end_time <= end_date)
                                select s.leave_id;

            bool hasLeave = leaveCheck.Any();
            if (!hasLeave)
            {
                decimal? noLeaveAwards = _db.Incentives
                .Where(i => i.company_id == currentStaffCompany)
                .Select(i => i.attendance_Excellence_Award)
                .FirstOrDefault();

                // Check if noLeaveAwards has a value before converting to float
                float floatNoLeaveAwards = noLeaveAwards.HasValue ? (float)noLeaveAwards.Value : 0.0f;
                incentive_total += floatNoLeaveAwards;
            }              
            var ratings = _db.Rating.Where(r => r.staff_rated == payroll.staff_id
                        && (r.rate_date >= start_date && r.rate_date <= end_date));

            if (ratings.Any())
            {
                var avgRating = ratings.Average(r => r.rating);
                avgRating = (float)Math.Round(avgRating, 2);

                if (avgRating > 4.2)
                {
                    decimal? goodRateAwards = _db.Incentives
                        .Where(i => i.company_id == currentStaffCompany)
                        .Select(i => i.good_rating_incentives)
                        .FirstOrDefault();

                    // Check if noLeaveAwards has a value before converting to float
                    float floatGoodRateAwards = goodRateAwards.HasValue ? (float)goodRateAwards.Value : 0.0f;
                    incentive_total += floatGoodRateAwards;
                }
            }

            if (incentivesID != null) {
                var newEmployeeIncentive = new EmployeeIncentives
                {
                    employee_id = payroll.staff_id,
                    incentives_id = incentivesID,
                    start_Claimed = start_date,
                    end_Claimed = end_date
                };
                _db.EmployeeIncentives.Add(newEmployeeIncentive);
                await _db.SaveChangesAsync();
            }
          
            

            //check kpi part
            var kpiRecord = _db.CompanyKPIs.Where(kpi => kpi.company_id == currentStaffCompany
                            && kpi.start_date >= start_date && kpi.end_date <= end_date);

            foreach (var kpi in kpiRecord)
            {
                var existingRecordKPI = _db.EmployeeKPI
               .FirstOrDefault(ei =>
                   ei.employee_id == payroll.staff_id &&
                   ei.KPI_id == kpi.KPI_id);

                if (existingRecordKPI == null)
                {
                    if (kpi.status == "Achieve")
                    {
                        decimal? KPIAwards = kpi.hit_KPI_allowance;

                        float hit_kpi = KPIAwards.HasValue ? (float)KPIAwards.Value : 0.0f;
                        incentive_total += hit_kpi;

                    }

                    EmployeeKPI ekpi = new EmployeeKPI()
                    {
                        employee_id = payroll.staff_id,
                        KPI_id = kpi.KPI_id
                    };

                    _db.EmployeeKPI.Add(ekpi);
                    await _db.SaveChangesAsync();
                }

            }
            payroll.incentives_total = incentive_total;
            payroll.start_date = start_date;
            payroll.end_date = end_date;

            // 
            var salaryDonePaid = _db.SalaryAdvance.Where(sa => sa.employee_id == payroll.staff_id 
            && sa.status != "Done pay back" && sa.status != "decline" && sa.status != "pending");

            foreach (var sa in salaryDonePaid)
            {
                if (sa != null)
                {
                    payroll.advance_id = sa.advance_id;
                    _db.SalaryAdvance.Update(sa);
                    await _db.SaveChangesAsync();
                }
            }

            //check pay back
            var salaryAdvancePaid = _db.SalaryAdvance.Where(sa => sa.employee_id == payroll.staff_id && (sa.status == "paid" || sa.status == "paying back"));


            foreach (var saPaid in salaryAdvancePaid)
            {
                if (saPaid != null)
                {
                    var result = from sa in _db.SalaryAdvance
                                 join pb in _db.PayBack
                                 on sa.advance_id equals pb.advance_id
                                 where sa.employee_id == payroll.staff_id && (sa.status == "paid" || sa.status == "paying back")
                                       && pb.status == "Not Paid"
                                       && sa.advance_id == pb.advance_id
                                 select new
                                 {
                                     SalaryAdvance = sa,
                                     PayBack = pb
                                 };

                    foreach (var payback in result)
                    {
                        if (payroll.month_salary > payback.PayBack.payback_amount)
                        {
                            if (payback.PayBack.status != "Paid")
                            {
                                payroll.payback_id = payback.PayBack.payback_id;

                                payback.PayBack.status = "Paid";
                                _db.PayBack.Update(payback.PayBack);
                                await _db.SaveChangesAsync();

                                break;
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "The salary is not enough to payback, please consider the longer time period";
                            return RedirectToAction(nameof(Index));
                        }                      
                    }


                    int countPayBack = 0;
                    var result2 = from sa in _db.SalaryAdvance
                                  join pb in _db.PayBack
                                  on sa.advance_id equals pb.advance_id
                                  where sa.employee_id == payroll.staff_id && (sa.status == "paid" || sa.status == "paying back")
                                        && pb.status == "Paid"
                                        && sa.advance_id == pb.advance_id
                                  select new
                                  {
                                      SalaryAdvance = sa,
                                      PayBack = pb
                                  };

                    List<PayBack> payBackList = result2.Select(result => result.PayBack).ToList();

                    countPayBack = payBackList.Count;

                        // done payback all the amount
                        if (countPayBack == saPaid.time_to_payback)
                    {
                        saPaid.status = "Done pay back";
                        _db.SalaryAdvance.Update(saPaid);
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        saPaid.status = "paying back";
                        _db.SalaryAdvance.Update(saPaid);
                        await _db.SaveChangesAsync();
                    }
                }
            }


            //check salary advance
            var salaryAdvance = _db.SalaryAdvance.Where(sa => sa.employee_id == payroll.staff_id && (sa.status == "approved"));

            foreach (var sa in salaryAdvance) 
            {
                if (sa != null)
                {
                    payroll.advance_id = sa.advance_id;

                    sa.status = "paid";

                    _db.SalaryAdvance.Update(sa);
                    await _db.SaveChangesAsync();
                }
            }


            // update the attendance claim
            for (int i = 0; i < shiftID.Length; i++)
            {
                var startTime = await (from s in _db.Shift where s.shift_id == shiftID[i] && s.shift_start >= start_date && s.shift_start <= end_date select s.shift_start).FirstOrDefaultAsync();
                var endTime = await (from s in _db.Shift where s.shift_id == shiftID[i] && s.shift_end <= end_date select s.shift_end).FirstOrDefaultAsync();

                var shiftToClaim = await
                    (from s in _db.Shift
                     where s.shift_id == shiftID[i]
                     && s.shift_start >= start_date && s.shift_start <= end_date
                     && s.shift_end <= end_date
                     select s.shift_id).FirstOrDefaultAsync();

                var attendanceClaim = await _db.Attendance.Where(ac => ac.shift_id == shiftToClaim && ac.claimed == false
                                          && ac.start_time >= start_date && ac.start_time <= end_date
                                      && ac.end_time <= end_date).FirstOrDefaultAsync();

                if (attendanceClaim != null)
                {
                    attendanceClaim.claimed = true;
                    _db.Attendance.Update(attendanceClaim);
                    await _db.SaveChangesAsync();
                }

            }

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
        //no use, after test by lkl
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

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _db.Payroll == null)
            {
                return NotFound();
            }

            var payroll = await _db.Payroll
                .Include(p => p.EmployeeDetails)
                .FirstOrDefaultAsync(m => m.payroll_id == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // POST: Staff/Payrolls2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_db.Payroll == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Payroll'  is null.");
            }
            var payroll = await _db.Payroll.FindAsync(id);

            if (payroll != null)
            {
               
                var empIncentives = await _db.EmployeeIncentives.Where(ei => ei.start_Claimed==payroll.start_date 
                                    && ei.end_Claimed==payroll.end_date && ei.employee_id == payroll.staff_id).ToListAsync();


                var kpiFound = await _db.CompanyKPIs.Where(c => c.start_date >= payroll.start_date && c.end_date <= payroll.end_date).ToListAsync();

                foreach (var kpi in kpiFound) {
                    var empKPI = await _db.EmployeeKPI.Where(e=> e.employee_id==payroll.staff_id && e.KPI_id == kpi.KPI_id).FirstOrDefaultAsync();
                    _db.EmployeeKPI.Remove(empKPI);
                }

                var shift = from s in _db.Attendance where s.staff_id == payroll.staff_id select s.shift_id;
                string[] shiftID = shift.ToArray();

                for (int i = 0; i < shiftID.Length; i++) {
                    var shiftToClaim = await
                                     (from s in _db.Shift
                                      where s.shift_id == shiftID[i]
                                      && s.shift_start >= payroll.start_date && s.shift_start <= payroll.end_date
                                      && s.shift_end <= payroll.end_date
                                      select s.shift_id).FirstOrDefaultAsync();

                    var attendanceClaim = await _db.Attendance.Where(ac => ac.shift_id == shiftToClaim && ac.claimed == true
                                              && ac.start_time >= payroll.start_date && ac.start_time <= payroll.end_date
                                          && ac.end_time <= payroll.end_date).FirstOrDefaultAsync();

                    if (attendanceClaim != null) {
                        attendanceClaim.claimed = false;
                        _db.Attendance.Update(attendanceClaim);
                    }
                }

                var salaryAdvance = await _db.SalaryAdvance.Where(sa => sa.advance_id == payroll.advance_id).FirstOrDefaultAsync();
                var payBack = await _db.PayBack.Where(pb => pb.advance_id == payroll.advance_id).ToListAsync();
                
                if (salaryAdvance!= null && payBack !=null)
                {
                    var findPayBack = await _db.PayBack.Where(pb => pb.advance_id == payroll.advance_id && pb.status == "Not Paid").ToListAsync();

                    // the salary advance is in paid only, have not start to payback, so change to approved status
                    var salaryAdvancePaid = await _db.SalaryAdvance.Where(sa => sa.advance_id == payroll.advance_id
                                            && sa.status=="paid").FirstOrDefaultAsync();
                    if (findPayBack.Count() == payBack.Count() && salaryAdvancePaid != null) {
                        salaryAdvancePaid.status = "approved";
                        _db.SalaryAdvance.Update(salaryAdvancePaid);
                        await _db.SaveChangesAsync();
                    }

                    // the salary advance in to paying back status
                    var salaryAdvancePayingBack = await _db.SalaryAdvance.Where(sa => sa.advance_id == payroll.advance_id
                                          && sa.status == "paying back").FirstOrDefaultAsync();
                    if (salaryAdvancePayingBack!=null) {
                        if (findPayBack.Count() < payBack.Count()) {

                            // find the last paid payBack and change it status to not paid
                            var findLastPayBack = _db.PayBack.Where(pb => pb.advance_id == payroll.advance_id && pb.status == "Paid").OrderByDescending(pb => pb.payback_id).FirstOrDefaultAsync();
                            findLastPayBack.Result.status = "Not Paid";
                            _db.PayBack.Update(findLastPayBack.Result);
                            await _db.SaveChangesAsync();
                        }

                        var findPayBackCheck = await _db.PayBack.Where(pb => pb.advance_id == payroll.advance_id && pb.status == "Not Paid").ToListAsync();

                        if (findPayBackCheck.Count() == payBack.Count())
                        {
                            salaryAdvancePayingBack.status = "paid";
                            _db.SalaryAdvance.Update(salaryAdvancePayingBack);
                            await _db.SaveChangesAsync();
                        }
                    }

                    var salaryAdvanceDonePaid = await _db.SalaryAdvance.Where(sa => sa.advance_id == payroll.advance_id
                      && sa.status == "Done pay back").FirstOrDefaultAsync();
                    if (salaryAdvanceDonePaid != null) {

                        // find the last paid payBack and change it status to not paid
                        var findLastPayBack = _db.PayBack.Where(pb => pb.advance_id == payroll.advance_id && pb.status == "Paid").OrderByDescending(pb => pb.payback_id).FirstOrDefaultAsync();
                        findLastPayBack.Result.status = "Not Paid";
                        _db.PayBack.Update(findLastPayBack.Result);
                        await _db.SaveChangesAsync();

                        salaryAdvanceDonePaid.status = "paying back";
                        _db.SalaryAdvance.Update(salaryAdvanceDonePaid);
                        await _db.SaveChangesAsync();

                    }
                }


                _db.EmployeeIncentives.RemoveRange(empIncentives);
                _db.Payroll.Remove(payroll);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<string> GeneratePayRollID()
        {

            var aspId = User.Identity?.Name;
            var currentUser = await _db.EmployeeDetails.Where(e => e.user_id == aspId).FirstOrDefaultAsync();

            string newId;
            string prefix = "PR" + currentUser.employee_id.ToString();

            // Retrieve the last question ID from the database
            var lastPayRoll = await _db.Payroll
                .OrderByDescending(pb => pb.payroll_id)
                .FirstOrDefaultAsync();

            if (lastPayRoll != null)
            {
                // Extract the numeric part of the last question ID and increment it by one
                int lastIdNumericPart = int.Parse(lastPayRoll.payroll_id.Substring(8));
                newId = prefix + (lastIdNumericPart + 1).ToString("");
            }
            else
            {
                // If no question exists, start with Q00001
                newId = prefix + "0";
            }

            return newId;
        }

    }
}
