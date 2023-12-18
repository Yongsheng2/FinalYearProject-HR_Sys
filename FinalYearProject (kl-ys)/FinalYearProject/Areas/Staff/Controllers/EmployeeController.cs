using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Text.Encodings.Web;
using System.Text;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEmailSender _emailSender;

        public EmployeeController(IEmailSender emailSender, UserManager<IdentityUser> userManager, ApplicationDbContext db , IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var aspId = User.Identity?.Name;

            if (aspId == null)
            {
                return RedirectToAction("Login", "Account", new {area = "Identity"});
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
                        if (currentAdmin.is_superadmin)
                        {
                            return View(await _db.EmployeeDetails.ToListAsync());
                        }
                        else
                        {
                            return View(await _db.EmployeeDetails.Where(e => e.Company.current_admin == currentAdmin.admin_id).ToListAsync());
                        }
                    }

                }
                else
                {
                    return View(await _db.EmployeeDetails.Where(e => e.employer_id == currentUser.employee_id || e.employee_id == currentUser.employee_id).ToListAsync());
                }
            }
        }

        public async Task<IActionResult> Details(string empId)
        {
            var employee = await _db.EmployeeDetails.FindAsync(empId);

            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                return View(employee);
            }
        }

        public async Task<IActionResult> Create()
        {
            var companies = await _db.Company.ToListAsync();
            ViewBag.parent_company = new SelectList(companies.AsEnumerable(), "company_id", "company_name");

            var roles = await _db.Role.ToListAsync();
            ViewBag.staff_role = new SelectList(roles.AsEnumerable(), "role_id", "role_name");

            List<string> genders = new List<string>
            {
                "Male",
                "Female"
            };

            ViewBag.gender = new SelectList(genders.AsEnumerable());

            List<string> nations = new List<string>
            {
                "Malaysia",
                "Singapore"
            };

            ViewBag.nationality = new SelectList(nations.AsEnumerable());

            EmployeeDetails newEmp = new EmployeeDetails()
            {
                employee_id = GenerateEmployeeID().Result,
                paidLeaveHourLeft = 140,
                paidLeaveOnBargain = 0,
                sickLeaveHourLeft = 140,
                sickLeaveOnBargain = 0,
                leaveUpdate = DateTime.Now
            };

            return View(newEmp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeDetails employeeDetails)
        {
            employeeDetails.types_of_wages = "Wages";
            employeeDetails.is_active = true;

            if (ModelState.IsValid)
            {
                var user = new IdentityUser();

                user.Email = employeeDetails.email;
                user.UserName = employeeDetails.user_id;

                var result = await _userManager.CreateAsync(user, employeeDetails.acc_pass);
                
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                    pageHandler: null,
                        values: new { userId = employeeDetails.user_id, code = code },
                        protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(
                        employeeDetails.email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    try
                    {
                        _db.EmployeeDetails.Add(employeeDetails);
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        var deleteResult = await _userManager.DeleteAsync(user);

                        if (deleteResult.Succeeded)
                        {
                            return View(employeeDetails);
                        }
                    }

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var userFromDb = await _db.EmployeeDetails.FindAsync(employeeDetails.employee_id);
                    
                    if (files.Count > 0)
                    {
                        var uploads = Path.Combine(webRootPath, "images");
                        var extension = Path.GetExtension(files[0].FileName);

                        using (var filestream = new FileStream(Path.Combine(uploads, employeeDetails.employee_id + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }

                        userFromDb.profileImg_path = @"\images\" + userFromDb.employee_id + extension;
                    }
                    else
                    {
                        var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultUserImage);
                        System.IO.File.Copy(uploads, webRootPath + @"\images\" + employeeDetails.employee_id + ".png");
                        userFromDb.profileImg_path = @"\images\" + employeeDetails.employee_id + ".png";
                    }

                    await _db.SaveChangesAsync();

                    await SetRolePermission(userFromDb.employee_id, userFromDb.staff_role);

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var companies = await _db.Company.ToListAsync();
            ViewBag.parent_company = new SelectList(companies.AsEnumerable(), "company_id", "company_name");

            var roles = await _db.Role.ToListAsync();
            ViewBag.staff_role = new SelectList(roles.AsEnumerable(), "role_id", "role_name");

            List<string> genders = new List<string>
            {
                "Male",
                "Female"
            };

            ViewBag.gender = new SelectList(genders.AsEnumerable());

            List<string> nations = new List<string>
            {
                "Malaysia",
                "Singapore"
            };

            ViewBag.nationality = new SelectList(nations.AsEnumerable());

            return View(employeeDetails);
        }

        public async Task<IActionResult> Edit(string? id)
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

            var roles = await _db.Role.ToListAsync();
            ViewBag.staff_role = new SelectList(roles.AsEnumerable(), "role_id", "role_name");

            var employers = await _db.EmployeeDetails.Where(e => e.parent_company == employeeDetails.parent_company && e.employee_id != employeeDetails.employee_id).ToListAsync();
            ViewBag.employer_id = new SelectList(employers.AsEnumerable(), "employee_id", "employee_name");

            List<string> genders = new List<string>
            {
                "Male",
                "Female"
            };

            ViewBag.gender = new SelectList(genders.AsEnumerable());

            List<string> nations = new List<string>
            {
                "Malaysia",
                "Singapore"
            };

            ViewBag.nationality = new SelectList(nations.AsEnumerable());

            List<string> religion = new List<string>
            {
                "Muslim",
                "Non-Muslim"
            };

            ViewBag.religion = new SelectList(religion.AsEnumerable());

            EmployeeEditVM edVM = new EmployeeEditVM()
            {
                employee = employeeDetails,
                user = employeeDetails.user_id
            };

            return View(edVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeEditVM empEdit)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FindAsync(empEdit.user);

                if (user != null)
                {
                    bool userChanged = false;

                    if (user.UserName != empEdit.employee.user_id)
                    {
                        user.UserName = empEdit.employee.user_id;
                        userChanged = true;
                    }

                    if (user.Email != empEdit.employee.email)
                    {
                        user.Email = empEdit.employee.email;

                        user.EmailConfirmed = false;

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                        pageHandler: null,
                            values: new { userId = empEdit.employee.user_id, code = code },
                            protocol: Request.Scheme);
                        await _emailSender.SendEmailAsync(
                            empEdit.employee.email,
                            "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        userChanged = true;
                    }

                    if (userChanged) 
                    {
                        _db.Update(user);
                        await _db.SaveChangesAsync();
                    }
                }

                _db.Update(empEdit.employee);
                await _db.SaveChangesAsync();

                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                var userFromDb = await _db.EmployeeDetails.FindAsync(empEdit.employee.employee_id);

                if (files.Count > 0)
                {
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var filestream = new FileStream(Path.Combine(uploads, empEdit.employee.employee_id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    userFromDb.profileImg_path = @"\images\" + userFromDb.employee_id + extension;
                }

                await _db.SaveChangesAsync();

                await SetRolePermission(userFromDb.employee_id, userFromDb.staff_role);

                return RedirectToAction(nameof(Index));
            }

            var companies = await _db.Company.ToListAsync();
            ViewBag.parent_company = new SelectList(companies.AsEnumerable(), "company_id", "company_name");

            var roles = await _db.Role.ToListAsync();
            ViewBag.staff_role = new SelectList(roles.AsEnumerable(), "role_id", "role_name");

            List<string> genders = new List<string>
            {
                "Male",
                "Female"
            };

            ViewBag.gender = new SelectList(genders.AsEnumerable());

            List<string> nations = new List<string>
            {
                "Malaysia",
                "Singapore"
            };

            ViewBag.nationality = new SelectList(nations.AsEnumerable());

            return View(empEdit);
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
            var employeeDetails = await _db.EmployeeDetails.FindAsync(id);

            if (employeeDetails == null)
            {
                return View();
            }

            employeeDetails.is_active = false;

            _db.EmployeeDetails.Update(employeeDetails);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        public async Task<string> GenerateEmployeeID()
        {
            string newId;
            string prefix = "E";

            var totalEmployee = await _db.EmployeeDetails.CountAsync();

            newId = prefix + (totalEmployee + 1).ToString("00000");

            return newId;
        }


        [ActionName("GenerateCompanyEmployeeID")]
        public async Task<IActionResult> GenerateCompanyEmployeeID(string companyID)
        {
            string newId;
            string prefix = companyID.ToString().Substring(0, 1);

            var totalEmployeeInCompany = await _db.EmployeeDetails.Where(e => e.parent_company == companyID).CountAsync();

            newId = prefix + (totalEmployeeInCompany + 1).ToString("00000");

            return Json(newId);
        }

        public async Task SetRolePermission(string empId, string roleId)
        {
            var employee = await _db.EmployeeDetails.FindAsync(empId);
            var role = await _db.Role.FindAsync(roleId);
            var user = await _userManager.FindByNameAsync(employee.user_id);

            var aspRole = await _db.Roles.ToListAsync();
            var permission = await _db.Permission.Where(p => p.baseRole == role.role_id).Select(p => p.identityRole).ToListAsync();

            if (user != null)
            {
                if (permission.Count > 0)
                {
                    foreach (IdentityRole identityRole in aspRole)
                    {
                        var isIn = await _userManager.IsInRoleAsync(user, identityRole.Name);

                        if (permission.Contains(identityRole.Name) && !isIn)
                        {
                            await _userManager.AddToRoleAsync(user, identityRole.Name);
                        }
                        else if (!permission.Contains(identityRole.Name) && isIn)
                        {
                            await _userManager.RemoveFromRoleAsync(user, identityRole.Name);
                        }
                    }
                }
            }
        }
    }
}
