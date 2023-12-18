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
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = SD.SalaryAdvanceManage)]
    public class SalaryAdvancesManageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalaryAdvancesManageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SalaryAdvancesManage
        public async Task<IActionResult> Index()
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
                    var applicationDbContext = _context.SalaryAdvance;
                    return View(await applicationDbContext.ToListAsync());

                }
            }


        }

        // GET: Admin/SalaryAdvancesManage/Details/5
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

        // GET: Admin/SalaryAdvancesManage/Create      
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
            ViewBag.status = new SelectList(new List<string> { "approved", "decline" });


            return View(salaryAdvance);
        }

        // POST: Admin/SalaryAdvancesManage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SalaryAdvance salaryAdvance)
        {
            if (id != salaryAdvance.advance_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
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
                            salaryAdvance.approved_by = currentUser.employee_id;
                            _context.Update(salaryAdvance);
                            await _context.SaveChangesAsync();


                            if (salaryAdvance.status == "approved")
                            {

                                for (int i = 0; i < salaryAdvance.time_to_payback; i++)
                                {
                                    PayBack pb = new PayBack()
                                    {
                                        payback_id = GeneratePayBackID().Result,
                                        advance_id = salaryAdvance.advance_id,
                                        status = "Not Paid",
                                        payback_amount = (float)(decimal)Math.Round(salaryAdvance.amount.Value / salaryAdvance.time_to_payback.Value, 2)
                                    };
                                    _context.Add(pb);
                                    await _context.SaveChangesAsync();
                                }

                            }

                        }
                    }

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

        // GET: Admin/SalaryAdvancesManage/Delete/5
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

        // POST: Admin/SalaryAdvancesManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SalaryAdvance == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SalaryAdvance'  is null.");
            }

            var payback = await _context.PayBack.Where(pb => pb.advance_id == id).ToListAsync();

            _context.PayBack.RemoveRange(payback);

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

        public async Task<string> GeneratePayBackID()
        {
            string newId;
            string prefix = "PB";

            // Retrieve the last question ID from the database
            var lastPayBack = await _context.PayBack
                .OrderByDescending(pb => pb.payback_id)
                .FirstOrDefaultAsync();

            if (lastPayBack != null)
            {
                // Extract the numeric part of the last question ID and increment it by one
                int lastIdNumericPart = int.Parse(lastPayBack.payback_id.Substring(2));
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
