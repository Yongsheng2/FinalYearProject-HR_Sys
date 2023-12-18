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
    [Authorize(Roles = SD.CompanyKPIs)]
    public class CompanyKPIsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyKPIsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CompanyKPIs
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
                    var applicationDbContext = await _context.CompanyKPIs.Where(cp => cp.company_id == currentUser.parent_company).ToListAsync();
                    return View(applicationDbContext);
                }
            }

        }

        // GET: Admin/CompanyKPIs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.CompanyKPIs == null)
            {
                return NotFound();
            }

            var companyKPI = await _context.CompanyKPIs
                .FirstOrDefaultAsync(m => m.KPI_id == id);
            if (companyKPI == null)
            {
                return NotFound();
            }

            return View(companyKPI);
        }

        // GET: Admin/CompanyKPIs/Create
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
                    var currentAdmin = await _context.Admin.Where(a => a.admin_id == aspId).FirstOrDefaultAsync();

                    if (currentAdmin == null)
                    {
                        return RedirectToAction("Login", "Account", new { area = "Identity" });
                    }
                    else
                    {
                        return View(await _context.CompanyKPIs.ToListAsync());
                    }

                }
                else
                {
                    CompanyKPI kpi = new CompanyKPI()
                    {
                        KPI_id = GenerateCompanyKPIID().Result,
                        company_id = currentUser.parent_company
                    };
                    ViewBag.StatusOptions = new List<string> { "Pending", "Achieve", "Fail" };
                    return View(kpi);
                }
            }
        }

        // POST: Admin/CompanyKPIs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyKPI companyKPI)
        {
            if (companyKPI.end_date < companyKPI.start_date)
            {
                var errorMessage = "Start date must be early then end date";

                // Pass the error message to the view
                ViewData["ErrorMessageDate"] = errorMessage;
                ViewBag.StatusOptions = new List<string> { "Pending", "Achieve", "Fail" };
                return View(companyKPI);
            }

            _context.Add(companyKPI);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/CompanyKPIs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.CompanyKPIs == null)
            {
                return NotFound();
            }

            var companyKPI = await _context.CompanyKPIs.FindAsync(id);
            if (companyKPI == null)
            {
                return NotFound();
            }
            ViewBag.StatusOptions = new List<string> { "Pending", "Achieve", "Fail" };
            return View(companyKPI);
        }

        // POST: Admin/CompanyKPIs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CompanyKPI companyKPI)
        {
            if (id != companyKPI.KPI_id)
            {
                return NotFound();
            }

            if (companyKPI.end_date < companyKPI.start_date)
            {
                var errorMessage = "Start date must be early then end date";

                // Pass the error message to the view
                ViewData["ErrorMessageDate"] = errorMessage;
                ViewBag.StatusOptions = new List<string> { "Pending", "Achieve", "Fail" };
                return View(companyKPI);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyKPI);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyKPIExists(companyKPI.KPI_id))
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
            ViewBag.StatusOptions = new List<string> { "Pending", "Achieve", "Fail" };
            return View(companyKPI);
        }

        // GET: Admin/CompanyKPIs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.CompanyKPIs == null)
            {
                return NotFound();
            }

            var companyKPI = await _context.CompanyKPIs
                .FirstOrDefaultAsync(m => m.KPI_id == id);
            if (companyKPI == null)
            {
                return NotFound();
            }

            return View(companyKPI);
        }

        // POST: Admin/CompanyKPIs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.CompanyKPIs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CompanyKPIs'  is null.");
            }
            var companyKPI = await _context.CompanyKPIs.FindAsync(id);
            if (companyKPI != null)
            {
                _context.CompanyKPIs.Remove(companyKPI);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyKPIExists(string id)
        {
            return (_context.CompanyKPIs?.Any(e => e.KPI_id == id)).GetValueOrDefault();
        }


        public async Task<string> GenerateCompanyKPIID()
        {
            string newId;
            string prefix = "CKPI";

            var ckpi = await _context.CompanyKPIs
             .OrderByDescending(kpi => kpi.KPI_id)
             .FirstOrDefaultAsync();

            if (ckpi != null)
            {
                int lastIdNumericPart = int.Parse(ckpi.KPI_id.Substring(4));
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
