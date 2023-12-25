using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class ApplyClaimController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ApplyClaimController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
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
                    var claimList = await _db.EmployeeClaim.Where(c => c.staff_id == currentUser.employee_id).ToListAsync();

                    EmployeeClaimListVM list = new EmployeeClaimListVM()
                    {
                        owner = currentUser,
                        claimApplied = claimList,
                        displayClaim = new Models.EmployeeClaim(),
                    };

                    return View(list);
                }
            }
        }

        public async Task<IActionResult> Apply(string id)
        {
            var employeeSelected = await _db.EmployeeDetails.FindAsync(id);

            if (employeeSelected == null)
            {
                return NotFound();
            }
            else
            {
                EmployeeClaim claim = new EmployeeClaim()
                {
                    staff_id = employeeSelected.employee_id,
                    date_apply = DateTime.Now,
                    claim_id = GenerateClaimID(employeeSelected.employee_id).Result,
                };

                return View(claim);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(EmployeeClaim claim, IFormFile claimFile)
        {
            var employeeSelected = await _db.EmployeeDetails.FindAsync(claim.staff_id);

            if (employeeSelected == null)
            {
                return NotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (claimFile != null)
                    {
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        var uploads = Path.Combine(webRootPath, "claims");
                        var extension = Path.GetExtension(claimFile.FileName);
                        var filepath = claim.claim_id + extension;

                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        var filePathToSave = Path.Combine(uploads, filepath);

                        using (var filestream = new FileStream(filePathToSave, FileMode.Create))
                        {
                            claimFile.CopyTo(filestream);
                        }

                        claim.claimFile = @"\claims\" + filepath;
                        await _db.SaveChangesAsync();
                    }

                    _db.EmployeeClaim.Add(claim);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(claim);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var claim = await _db.EmployeeClaim.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            EmployeeEditClaimVM editVM = new EmployeeEditClaimVM
            {
                claim = claim,
                claimAmount = claim.claimAmount,
                claim_reason = claim.claim_reason
            };

            return View(editVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeEditClaimVM editClaim, IFormFile? claimFile = null)
        {
            var existingClaim = await _db.EmployeeClaim.FindAsync(editClaim.claim.claim_id);

            if (existingClaim == null)
            {
                return NotFound();
            }

            existingClaim.claimAmount = editClaim.claimAmount;
            existingClaim.claim_reason = editClaim.claim_reason;

            if (claimFile != null && claimFile.Length > 0)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var uploads = Path.Combine(webRootPath, "claims");
                var extension = Path.GetExtension(claimFile.FileName);
                var filepath = existingClaim.claim_id + extension;

                using (var filestream = new FileStream(Path.Combine(uploads, filepath), FileMode.Create))
                {
                    claimFile.CopyTo(filestream);
                }

                existingClaim.claimFile = @"\claims\" + filepath;
            }

            _db.Update(existingClaim);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var claim = await _db.EmployeeClaim.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }
            else
            {
                if (claim.approval_status != null)
                {
                    return View(claim);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var claim = await _db.EmployeeClaim.FindAsync(id);

            if (claim == null)
            {
                return View();
            }

            _db.EmployeeClaim.Remove(claim);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<string> GenerateClaimID(string empId)
        {
            string newId;
            string prefix = "C";

            var totalClaims = await _db.EmployeeClaim.CountAsync();

            newId = prefix + (totalClaims + 1).ToString("00000");

            return newId;
        }
    }
}
