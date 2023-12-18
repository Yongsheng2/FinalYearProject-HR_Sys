using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Authorization;


namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class SalaryAdvancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalaryAdvancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff/SalaryAdvances
        public async Task<IActionResult> Index()
        {
            var errorMessage = TempData["ErrorMessage"] as string;
            ViewData["ErrorMessage"] = errorMessage;

            var aspId = User.Identity?.Name;
            if (aspId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            else
            {
                var currentUser = await _context.EmployeeDetails.Where(e => e.user_id == aspId).FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    var applicationDbContext = _context.SalaryAdvance.Include
                        (s => s.ApprovedEmployeeDetails).Include(s => s.EmployeeDetails).Where(s => s.employee_id==currentUser.employee_id);
                    return View(await applicationDbContext.ToListAsync());

                }
            }
           
        }

        // GET: Staff/SalaryAdvances/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.SalaryAdvance == null)
            {
                return NotFound();
            }

            var salaryAdvance = await _context.SalaryAdvance
                .Include(s => s.ApprovedEmployeeDetails)
                .Include(s => s.EmployeeDetails)
                .FirstOrDefaultAsync(m => m.advance_id == id);
            if (salaryAdvance == null)
            {
                return NotFound();
            }

            return View(salaryAdvance);
        }

        // GET: Staff/SalaryAdvances/Create
        public async Task<IActionResult> Create()
        {

            var aspId = User.Identity?.Name;
            if (aspId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            else
            {
                var currentUser = await _context.EmployeeDetails.Where(e => e.user_id == aspId).FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    var checkAdvance = await _context.SalaryAdvance.Where(ca => ca.status == "pending" && ca.employee_id == currentUser.employee_id).ToListAsync();

                    if (checkAdvance.Count() == 1) {
                        TempData["ErrorMessage"] = "Only can apply one advance at a time, wait for admin to decide your advance";
                        return RedirectToAction(nameof(Index));
                    }

                    var checkAdvance2 = await _context.SalaryAdvance.Where(ca => ca.status!="pending" 
                                        && ca.status != "decline"
                                        && ca.status != "Done pay back" && ca.employee_id == currentUser.employee_id).ToListAsync();
                    
                    if (checkAdvance2.Count() < 1)
                    {
                        //check performance in kpi
                        //check kpi part
                        var kpiRecord = _context.CompanyKPIs.Where(kpi => kpi.company_id == currentUser.parent_company && kpi.status == "Achieve");
                        var result = from kpi in _context.CompanyKPIs
                                     join employee in _context.EmployeeDetails
                                     on kpi.company_id equals employee.parent_company
                                     where kpi.company_id == currentUser.parent_company
                                           && kpi.status == "Achieve"
                                           && employee.employee_id == currentUser.employee_id
                                     select new
                                     {
                                         CompanyKPI = kpi,
                                         Employee = employee
                                         // Include other properties as needed
                                     };

                        int countKPI = 0;

                        // check all rating for a employee
                        var ratings = _context.Rating.Where(r => r.staff_rated == currentUser.employee_id);
                        var avgRating = 0.00;

                        if (ratings.Any())
                        {
                            avgRating = ratings.Average(r => r.rating);
                            avgRating = (float)Math.Round(avgRating, 2);
                        }

                        var leaveCheck = _context.Attendance.Where(s => s.staff_id == currentUser.employee_id);
                        int countLeave = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            int currentMonth = DateTime.Now.Month;

                            currentMonth -= i;


                            foreach (var lc in leaveCheck)
                            {
                                if (lc.start_time != null && lc.start_time.Value.Month == currentMonth && lc.leave_id != null)
                                {
                                    // This leave record is in the desired month
                                    countLeave++;
                                }
                            }



                            foreach (var kpi in result)
                            {
                                if (kpi.CompanyKPI.start_date.Value.Month == currentMonth)
                                {
                                    // This leave record is in the desired month
                                    countKPI++;
                                }
                            }
                        }


                        if (countKPI > 1 && avgRating >= 4.2 && countLeave <= 3)
                        {
                            DateTime now = DateTime.Now;
                            SalaryAdvance sa = new SalaryAdvance()
                            {
                                advance_id = GenerateSalaryAdvanceID().Result,
                                employee_id = currentUser.employee_id,
                                request_date = now,
                                status = "pending"

                            };

                            ViewBag.amount = new SelectList(new List<float> { 500, 1000, 1500 });
                            ViewBag.time_to_payback = new SelectList(new List<int> { 1, 2, 3 });

                            return View(sa);
                        }
                        else
                        {
                            string errorMessage = "You do not fulfill the requirement to request a salary advance.<br>";

                            if (avgRating < 4.2)
                            {
                                errorMessage += "&nbsp;&nbsp; - Rating is below 4.2. <br>";
                            }

                            if (countLeave > 3)
                            {
                                errorMessage += "&nbsp;&nbsp; - Take more than 3 leaves in the past 6 months.<br>";
                            }

                            if (countKPI <= 1)
                            {
                                errorMessage += "&nbsp;&nbsp; - Not hit any KPI before.<br>";
                            }

                            TempData["ErrorMessage"] = errorMessage;

                            return RedirectToAction(nameof(Index));

                        }
                    }
                    else {
                        TempData["ErrorMessage"] = "Only can apply another advance unless you have done pay back your advance";
                        return RedirectToAction(nameof(Index));
                    }
                 
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalaryAdvance salaryAdvance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaryAdvance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }       
            return View(salaryAdvance);
        }

        // GET: Staff/SalaryAdvances/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SalaryAdvance == null)
            {
                return NotFound();
            }

            var salaryAdvance = await _context.SalaryAdvance.FindAsync(id);
            if (salaryAdvance == null)
            {
                return NotFound();
            }
            ViewBag.amount = new SelectList(new List<float> { 500, 1000, 1500 });
            ViewBag.time_to_payback = new SelectList(new List<int> { 1, 2, 3 });
            return View(salaryAdvance);
        }

        // POST: Staff/SalaryAdvances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("advance_id,employee_id,reason,amount,time_to_payback,status,request_date,approved_by")] SalaryAdvance salaryAdvance)
        {
            if (id != salaryAdvance.advance_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaryAdvance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryAdvanceExists(salaryAdvance.advance_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(salaryAdvance);
        }

        // GET: Staff/SalaryAdvances/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.SalaryAdvance == null)
            {
                return NotFound();
            }

            var salaryAdvance = await _context.SalaryAdvance
                .Include(s => s.ApprovedEmployeeDetails)
                .Include(s => s.EmployeeDetails)
                .FirstOrDefaultAsync(m => m.advance_id == id);
            if (salaryAdvance == null)
            {
                return NotFound();
            }

            return View(salaryAdvance);
        }

        // POST: Staff/SalaryAdvances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SalaryAdvance == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SalaryAdvance'  is null.");
            }
            var salaryAdvance = await _context.SalaryAdvance.FindAsync(id);
            if (salaryAdvance != null)
            {
                _context.SalaryAdvance.Remove(salaryAdvance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryAdvanceExists(string id)
        {
          return (_context.SalaryAdvance?.Any(e => e.advance_id == id)).GetValueOrDefault();
        }

        public async Task<string> GenerateSalaryAdvanceID()
        {
            string newId;
            string prefix = "A";

            // Retrieve the last question ID from the database
            var lastAdvance = await _context.SalaryAdvance
                .OrderByDescending(s => s.advance_id)
                .FirstOrDefaultAsync();

            if (lastAdvance != null)
            {
                // Extract the numeric part of the last question ID and increment it by one
                int lastIdNumericPart = int.Parse(lastAdvance.advance_id.Substring(1));
                newId = prefix + (lastIdNumericPart + 1).ToString("00000");
            }
            else
            {
                // If no question exists, start with Q00001
                newId = prefix + "00001";
            }

            return newId;
        }
    }
}
