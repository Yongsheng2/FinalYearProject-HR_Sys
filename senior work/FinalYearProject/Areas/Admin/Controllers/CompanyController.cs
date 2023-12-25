using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CompanyController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Company.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            var admins = await _db.Admin.ToListAsync();
            ViewBag.current_admin = new SelectList(admins.AsEnumerable(), "admin_id", "admin_id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            company.date_created = DateTime.Now;

            if (ModelState.IsValid)
            {
                _db.Company.Add(company);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _db.Company.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            var admins = await _db.Admin.ToListAsync();
            ViewBag.current_admin = new SelectList(admins.AsEnumerable(), "admin_id", "admin_id"); ;

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                _db.Update(company);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeDetails = await _db.EmployeeDetails.FindAsync(id);

            if (employeeDetails == null)
            {
                return NotFound();
            }

            return View(employeeDetails);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            var company = await _db.Company.FindAsync(id);

            if (company == null)
            {
                return View();
            }

            _db.Company.Remove(company);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }


}
