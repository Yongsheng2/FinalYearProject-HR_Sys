using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FinalYearProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            
            return View(await _db.Admin.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Admin admin)
        {
            var adminFromDb =  await _db.Company.FindAsync(admin.admin_id);

            if (ModelState.IsValid)
            {

                var user = new IdentityUser();

                user.UserName = admin.admin_id;

                var result = await _userManager.CreateAsync(user, admin.admin_pass);

                if (result.Succeeded)
                {
                    if (admin.is_superadmin)
                    {
                        await _userManager.AddToRoleAsync(user, SD.SuperAdmin);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Admin);
                    }


                    _db.Admin.Add(admin);
                    await _db.SaveChangesAsync();

                    

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
           

                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _db.Admin.FindAsync(id);

            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string? id)
        {
            var admin = await _db.Admin.FindAsync(id);

            if (admin == null)
            {
                return View();
            }

            _db.Admin.Remove(admin);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
