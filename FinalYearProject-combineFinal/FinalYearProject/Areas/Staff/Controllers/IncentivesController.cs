using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalYearProject.Data;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Authorization;
using FinalYearProject.Models;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = SD.Incentives)]
    public class IncentivesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncentivesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Incentives
        public async Task<IActionResult> Index()
        {            
            var aspId = User.Identity?.Name;
            var errorMessage = TempData["ErrorMessage"] as string;
            ViewData["ErrorMessage"] = errorMessage;

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
                    var applicationDbContext = _context.Incentives.Where(i => i.company_id == currentUser.parent_company);
                    return View(await applicationDbContext.ToListAsync());
                }
            }
        }

        // GET: Admin/Incentives/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Incentives == null)
            {
                return NotFound();
            }

            var incentives = await _context.Incentives
                .Include(i => i.Company)
                .FirstOrDefaultAsync(m => m.incentives_id == id);
            if (incentives == null)
            {
                return NotFound();
            }

            return View(incentives);
        }

        // GET: Admin/Incentives/Create
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
                        return View(await _context.Incentives.ToListAsync());
                    }

                }
                else
                {
                    Incentives incentives = new Incentives()
                    {
                        incentives_id = GenerateIncentivesID().Result,
                        company_id = currentUser.parent_company
                    };
                    return View(incentives);
                }
            }
        }
        // POST: Admin/Incentives/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("incentives_id,company_id,good_rating_incentives,attendance_Excellence_Award")] Incentives incentives)
        {
            if (ModelState.IsValid)
            {
                _context.Add(incentives);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["company_id"] = new SelectList(_context.Company, "company_id", "company_id", incentives.company_id);
            return View(incentives);
        }

        // GET: Admin/Incentives/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Incentives == null)
            {
                return NotFound();
            }

            var incentives = await _context.Incentives.FindAsync(id);
            if (incentives == null)
            {
                return NotFound();
            }
            ViewData["company_id"] = new SelectList(_context.Company, "company_id", "company_id", incentives.company_id);
            return View(incentives);
        }

        // POST: Admin/Incentives/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("incentives_id,company_id,good_rating_incentives,attendance_Excellence_Award")] Incentives incentives)
        {
            if (id != incentives.incentives_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incentives);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncentivesExists(incentives.incentives_id))
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
            ViewData["company_id"] = new SelectList(_context.Company, "company_id", "company_id", incentives.company_id);
            return View(incentives);
        }

        // GET: Admin/Incentives/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Incentives == null)
            {
                return NotFound();
            }

            var incentives = await _context.Incentives
                .Include(i => i.Company)
                .FirstOrDefaultAsync(m => m.incentives_id == id);
            if (incentives == null)
            {
                return NotFound();
            }

            return View(incentives);
        }

        // POST: Admin/Incentives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (_context.Incentives == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Incentives' is null.");
                }

                var incentives = await _context.Incentives.FindAsync(id);

                if (incentives != null)
                {
                    _context.Incentives.Remove(incentives);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "The is claimed by employee , cannot delete";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool IncentivesExists(string id)
        {
            return (_context.Incentives?.Any(e => e.incentives_id == id)).GetValueOrDefault();
        }


        public async Task<string> GenerateIncentivesID()
        {
            string newId;
            string prefix = "I";

            // Retrieve the last question ID from the database
            var lastIncentives = await _context.Incentives
                .OrderByDescending(i => i.incentives_id)
                .FirstOrDefaultAsync();

            if (lastIncentives != null)
            {
                // Extract the numeric part of the last question ID and increment it by one
                int lastIdNumericPart = int.Parse(lastIncentives.incentives_id.Substring(1));
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
