using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RatingController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _db = db;
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
                var currentUser = await _db.EmployeeDetails.Where(e => e.user_id == aspId).FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    var employeeUnder = await _db.EmployeeDetails.Where(e => e.employer_id == currentUser.employee_id).ToListAsync();
                    var employer = await _db.EmployeeDetails.Where(e => e.employee_id == currentUser.employer_id).FirstOrDefaultAsync();

                    RatingListVM rateVM = new RatingListVM();

                    List<EmployeeRatingVM> employeeRatings = new List<EmployeeRatingVM>();

                    foreach (EmployeeDetails emp in employeeUnder)
                    {
                        var ratings = await _db.Rating.Where(r => r.staff_rated == emp.employee_id).ToListAsync();

                        int count = 0, total = 0;
                        float avg = 0;

                        if (ratings.Count > 0)
                        {
                            foreach (Rating rt in ratings)
                            {
                                count++;
                                total += rt.rating;
                            }

                            avg = total / count;
                        }
                      

                        EmployeeRatingVM empRating = new EmployeeRatingVM()
                        {
                            employee = emp,
                            averageRate = avg,
                            rating = ratings,
                            count = count
                        };

                        employeeRatings.Add(empRating);
                    }

                    if (employer != null)
                    {
                        var ratings = await _db.Rating.Where(r => r.staff_rated == employer.employee_id).ToListAsync();

                        int count = 0, total = 0;
                        float avg = 0;

                        if (ratings.Count > 0)
                        {
                            foreach (Rating rt in ratings)
                            {
                                count++;
                                total += rt.rating;
                            }

                            avg = total / count;
                        }
                        


                        EmployeeRatingVM empRating = new EmployeeRatingVM()
                        {
                            employee = employer,
                            averageRate = avg,
                            rating = ratings,
                            count = count
                        };

                        rateVM.employerRating = empRating;
                    }

                    rateVM.employeesRating = employeeRatings;

                    return View(rateVM);
                }
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            var empSelected = await _db.EmployeeDetails.FindAsync(id);

            var ratings = await _db.Rating.Where(r => r.staff_rated == empSelected.employee_id).ToListAsync();

            int count = 0, total = 0;
            float avg = 0;

            if (ratings.Count > 0)
            {
                foreach (Rating rt in ratings)
                {
                    count++;
                    total += rt.rating;
                }

                avg = total / count;
            }

            EmployeeRatingVM empRating = new EmployeeRatingVM()
            {
                employee = empSelected,
                averageRate = avg,
                rating = ratings,
                count = count
            };
                        
            return View(empRating);
        }

        public async Task<IActionResult> Create(string id)
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
                    var rated = await _db.EmployeeDetails.FindAsync(id);
                    
                    var rating = await _db.Rating.Where(r => r.rated_by == currentUser.employee_id && r.staff_rated == rated.employee_id).FirstOrDefaultAsync();

                    if (rating == null)
                    {
                        Rating newRating = new Rating()
                        {
                            staff_rated = rated.employee_id,
                            rating_id = GenerateRatingID(rated.employee_id).Result,
                            rated_by = currentUser.employee_id
                        };

                        return View(newRating);
                    }
                    else
                    {
                        return RedirectToAction("Edit", "Rating", new { id });
                    }
                   
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rating rate)
        {
            if (ModelState.IsValid)
            {
                _db.Rating.Add(rate);
                await _db.SaveChangesAsync();

                return RedirectToAction("Details", new { id = rate.staff_rated });
            }
            else
            {
                return View(rate);
            }
        }

        public async Task<IActionResult> Edit(string id)
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
                    var rated = await _db.EmployeeDetails.FindAsync(id);

                    var rating = await _db.Rating.Where(r => r.rated_by == currentUser.employee_id && r.staff_rated == rated.employee_id).FirstOrDefaultAsync();

                    if (rating == null)
                    {
                        return RedirectToAction("Create", "Rating", new { id });
                    }
                    else
                    {
                        return View(rating);
                    }
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Rating rate)
        {
            if (ModelState.IsValid)
            {
                _db.Rating.Update(rate);
                await _db.SaveChangesAsync();

                return RedirectToAction("Details", new { id = rate.staff_rated });
            }
            else
            {
                return View(rate);
            }

        }

        public async Task<string> GenerateRatingID(string empId)
        {
            var totalRate = await _db.Rating.Where(r => r.staff_rated == empId).CountAsync();
            string prefix = empId + "-R";

            string newID = prefix + (totalRate + 1).ToString("00000");

            return newID;
        }
    }
}
