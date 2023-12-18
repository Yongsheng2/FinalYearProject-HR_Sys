using FinalYearProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class ApproveClaimController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ApproveClaimController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
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
                var currentAdmin = await _db.Admin.Where(a => a.admin_id == aspId).FirstOrDefaultAsync();

                if (currentAdmin == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    var claimList = currentAdmin != null
                        ? await _db.EmployeeClaim.ToListAsync()
                        : await (from claim in _db.EmployeeClaim
                                 join emp in _db.EmployeeDetails on claim.staff_id equals emp.employee_id
                                 join com in _db.Company on emp.parent_company equals com.company_id
                                 where com.current_admin == currentAdmin.admin_id
                                 select claim).ToListAsync();

                    return View(claimList);
                }
            }
        }

        public async Task<IActionResult> Decide(string id)
        {
            List<string> status = new List<string>();
            status.Add("Approved");
            status.Add("Rejected");

            ViewBag.status = new SelectList(status.AsEnumerable());

            var claim = await _db.EmployeeClaim.FindAsync(id);

            claim.reject_reason = "-";

            if (claim.approval_status != "")
            {
                if (claim == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(claim);
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decide([Bind("claim_id, staff_id, approval_status, claim_reason, date_apply, reject_reason, claimAmount, claimFile")] FinalYearProject.Models.EmployeeClaim claim)
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
                try
                {
                    var existingClaim = await _db.EmployeeClaim.FindAsync(claim.claim_id);

                    if (existingClaim != null)
                    {
                        existingClaim.approval_status = string.IsNullOrWhiteSpace(claim.approval_status) ? "Pending" : claim.approval_status;
                        existingClaim.reject_reason = claim.reject_reason;

                        _db.Update(existingClaim);
                        await _db.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while processing your request.";
                    return View(claim); 
                }
            }

            return View(claim);
        }

        public async Task<FileResult> Download(string? id)
        {
            var claim = await _db.EmployeeClaim.FindAsync(id);

            string path = _hostingEnvironment.WebRootPath + claim.claimFile;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", claim.staff_id + "_" + claim.claim_id + Path.GetExtension(path));
        }
    }
}