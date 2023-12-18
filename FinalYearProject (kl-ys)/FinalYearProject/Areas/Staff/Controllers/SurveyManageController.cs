using EllipticCurve.Utils;
using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using SendGrid.Helpers.Mail;
using System;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = SD.SurveyManage)]
    public class SurveyManageController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public SurveyManageController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }
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
                var currentUser = await _db.EmployeeDetails.Where(e => e.user_id == aspId).FirstOrDefaultAsync();

                return View(await _db.Survey.Where(s=>s.company_id==currentUser.parent_company).ToListAsync());
            }

               
        }

        public async Task<IActionResult> Create()
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
                    //return View(await _db.EmployeeDetails.Where(e => e.employer_id == currentUser.employee_id || e.employee_id == currentUser.employee_id).ToListAsync());

                    Survey survey = new Survey()
                    {
                        survey_id = GenerateSurveyID().Result,
                        company_id = currentUser.parent_company
                    };


                    return View(survey);
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Survey survey)
        {
            if (ModelState.IsValid)
            {

                return RedirectToAction(nameof(Index), new
                {
                    area = "Admin",
                    controller = "SurveyQuestions",
                    survey.survey_id
                    ,
                    survey.company_id,
                    survey.survey_name,
                    survey
                });
            }
            return View(survey);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            var checkSurvey = await _db.SurveyTaken.Where(st => st.survey_id == id).ToListAsync();
            if (checkSurvey.Count() == 0)
            {
                var surveyQuestions = await _db.SurveyQuestion
                .Where(sq => sq.survey_id == id)
                .Include(sq => sq.Question) // find the question in the survey
                .ToListAsync();


                var questions = surveyQuestions.Select(sq => sq.Question).ToList();

                List<ViewSurveyQuestionVM> surveyQuestion = new List<ViewSurveyQuestionVM>()
            {
                new ViewSurveyQuestionVM(){
                    survey_id = id,
                    questionSet = questions,
                    questionSetType = await _db.Question.Include(q => q.questionType).ToListAsync(),
                     survey_name = await _db.Survey.Where(s=>s.survey_id==id).Select(s=>s.survey_name).FirstOrDefaultAsync()
                }

            };

                return View(surveyQuestion);
            }
            else
            {
                TempData["ErrorMessage"] = "The survey are be answered by staff, can not be modify";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> View(string? id)
        {

            var surveyQuestions = await _db.SurveyQuestion
            .Where(sq => sq.survey_id == id)
            .Include(sq => sq.Question) // find the question in the survey
            .ToListAsync();


            var questions = surveyQuestions.Select(sq => sq.Question).ToList();

            List<ViewSurveyQuestionVM> surveyQuestion = new List<ViewSurveyQuestionVM>()
                {
                    new ViewSurveyQuestionVM(){
                        survey_id = id,
                        questionSet = questions,
                        questionSetType = await _db.Question.Include(q => q.questionType).ToListAsync(),
                        survey_name = await _db.Survey.Where(s=>s.survey_id==id).Select(s=>s.survey_name).FirstOrDefaultAsync()
                    }

                };

            // Pass the questions to your view
            return View(surveyQuestion);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Survey survey)
        {
            if (ModelState.IsValid)
            {
                _db.Update(survey);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Delete(string? id)
        {

            var surveyQuestions = await _db.SurveyQuestion
                .Where(sq => sq.survey_id == id)
                .Include(sq => sq.Question) // find the question in the survey
                .ToListAsync();


            var questions = surveyQuestions.Select(sq => sq.Question).ToList();

            List<ViewSurveyQuestionVM> surveyQuestion = new List<ViewSurveyQuestionVM>()
            {
                new ViewSurveyQuestionVM(){
                    survey_id = id,
                    questionSet = questions,
                    questionSetType = await _db.Question.Include(q => q.questionType).ToListAsync()
                }

            };
            // Pass the questions to your view
            return View(surveyQuestion);


        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            var survey = await _db.Survey.FindAsync(id);

            if (survey == null)
            {
                return View();
            }
            var beTaken = await _db.SurveyTaken.Where(st => st.survey_id == id).FirstOrDefaultAsync();

            if (beTaken == null)
            {
                _db.Survey.Remove(survey);
                await _db.SaveChangesAsync();

                //delete survey question also
                // Find all SurveyQuestion entries with the specified survey_id
                var surveyQuestionsToDelete = await _db.SurveyQuestion
                    .Where(sq => sq.survey_id == id)
                    .ToListAsync();

                // Remove the found entries from the DbSet
                _db.SurveyQuestion.RemoveRange(surveyQuestionsToDelete);

                // Save changes to the database
                await _db.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "The survey are be answered by staff, can not be delete";
            }



            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteQuestion(string? id)
        {

            var surveyQuestions = await _db.SurveyQuestion
                .Where(sq => sq.survey_id == id)
                .Include(sq => sq.Question) // find the question in the survey
                .ToListAsync();


            var questions = surveyQuestions.Select(sq => sq.Question).ToList();

            List<ViewSurveyQuestionVM> surveyQuestion = new List<ViewSurveyQuestionVM>()
            {
                new ViewSurveyQuestionVM(){
                    survey_id = id,
                    questionSet = questions,
                     questionSetType = await _db.Question.Include(q => q.questionType).ToListAsync(),
                      survey_name = await _db.Survey.Where(s=>s.survey_id==id).Select(s=>s.survey_name).FirstOrDefaultAsync()
                }

            };
            // Pass the questions to your view
            return View(surveyQuestion);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(List<ViewSurveyQuestionVM> surveyQuestion,
            string[] selectedQuestions, string[] allQuestionSurvey, string survey_id)
        {
            var result = from q in _db.Question
                         join surQ in _db.SurveyQuestion on q.question_id equals surQ.question_id
                         join qt in _db.QuestionType on q.questionType_id equals qt.questionType_id
                         where surQ.survey_id == survey_id
                         select new
                         {
                             Question = q,
                             QuestionType = qt
                         };

            var questionType = await _db.QuestionType.ToListAsync();
            var allQuestion = await _db.Question.ToListAsync();

            foreach (var type in questionType)
            {
                int countOri = 0;
                int countDelete = 0;
                foreach (var allQ in allQuestion)
                {
                    if (allQ.questionType_id == type.questionType_id)
                    {
                        for (int i = 0; i < allQuestionSurvey.Length; i++)
                        {
                            if (allQuestionSurvey[i] == allQ.question_id)
                            {
                                countOri++;
                            }
                        }
                    }
                }

                for (int i = 0; i < selectedQuestions.Length; i++)
                {
                    var findType = await _db.Question.Where(q => q.question_id == selectedQuestions[i]).FirstOrDefaultAsync();

                    if (findType.questionType_id == type.questionType_id)
                    {
                        for (int j = 0; j < allQuestionSurvey.Length; j++)
                        {
                            if (allQuestionSurvey[j] == selectedQuestions[i])
                            {
                                countDelete++;
                            }
                        }
                    }

                }
                if (allQuestionSurvey.Length != 0)
                {
                    if (countOri - countDelete <= 2)
                    {
                        TempData["ErrorMessage"] = "Every type of question must be ramain at least three after delete";

                        var surveyQuestionsError = await _db.SurveyQuestion
                       .Where(sq => sq.survey_id == survey_id)
                       .Include(sq => sq.Question) // find the question in the survey
                       .ToListAsync();

                        var questionsError = surveyQuestionsError.Select(sq => sq.Question).ToList();

                        List<ViewSurveyQuestionVM> sqError = new List<ViewSurveyQuestionVM>()
                    {
                        new ViewSurveyQuestionVM(){
                            survey_id = survey_id,
                            questionSet = questionsError,
                             questionSetType = await _db.Question.Include(q => q.questionType).ToListAsync(),
                             survey_name = await _db.Survey.Where(s=>s.survey_id==survey_id).Select(s=>s.survey_name).FirstOrDefaultAsync()
                        }

                    };
                        var errorMessage = TempData["ErrorMessage"] as string;
                        ViewData["ErrorMessage"] = errorMessage;
                        return View(sqError);
                    }
                }

            }


            foreach (var questionId in selectedQuestions)
            {
                var surveyQuestionEntry = new SurveyQuestion
                {
                    survey_id = survey_id,
                    question_id = questionId
                };
                // Mark the entity as deleted
                _db.SurveyQuestion.Remove(surveyQuestionEntry);
            }

            await _db.SaveChangesAsync();


            var surveyQuestions = await _db.SurveyQuestion
                    .Where(sq => sq.survey_id == survey_id)
                    .Include(sq => sq.Question) // find the question in the survey
                    .ToListAsync();

            var questions = surveyQuestions.Select(sq => sq.Question).ToList();

            List<ViewSurveyQuestionVM> sq = new List<ViewSurveyQuestionVM>()
            {
                new ViewSurveyQuestionVM(){
                    survey_id = survey_id,
                    questionSet = questions,
                     questionSetType = await _db.Question.Include(q => q.questionType).ToListAsync(),
                       survey_name = await _db.Survey.Where(s=>s.survey_id==survey_id).Select(s=>s.survey_name).FirstOrDefaultAsync()
                }

            };
            // Pass the questions to your view
            return View(sq);

        }


        public async Task<IActionResult> AddQuestion(string? id)
        {
            var existingQuestionIds = await _db.SurveyQuestion
                .Where(sq => sq.survey_id == id)
                .Select(sq => sq.question_id)
                .ToListAsync();

            var questionsNotInSurvey = await _db.Question
                .Where(q => !existingQuestionIds.Contains(q.question_id))
                .ToListAsync();


            // Create a ViewModel to pass the survey_id and the list of questions not in the survey
            ViewSurveyQuestionVM surveyQuestionVM = new ViewSurveyQuestionVM()
            {
                survey_id = id,
                questionSet = questionsNotInSurvey,
                questionSetType = await _db.Question.Include(q => q.questionType).ToListAsync(),
                survey_name = await _db.Survey.Where(s => s.survey_id == id).Select(s => s.survey_name).FirstOrDefaultAsync()
            };

            return View(surveyQuestionVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(string[] selectedQuestions, string survey_id)
        {
            SurveyQuestion surveyQuestion = new SurveyQuestion();

            surveyQuestion.survey_id = survey_id;


            foreach (var questionId in selectedQuestions)
            {
                var surveyQuestionEntry = new SurveyQuestion
                {
                    survey_id = survey_id,
                    question_id = questionId
                };

                _db.SurveyQuestion.Add(surveyQuestionEntry);
            }

            await _db.SaveChangesAsync();

            var existingQuestionIds = await _db.SurveyQuestion
                .Where(sq => sq.survey_id == survey_id)
                .Select(sq => sq.question_id)
                .ToListAsync();

            var questionsNotInSurvey = await _db.Question
                .Where(q => !existingQuestionIds.Contains(q.question_id))
                .ToListAsync();

            var questionTypesNotInSurvey = await _db.Question
                .Where(q => !existingQuestionIds.Contains(q.question_id))
                .Include(q => q.questionType)
                .Distinct()
                .ToListAsync();

            if (questionsNotInSurvey.Count == 0)
            {
                TempData["ErrorMessage"] = "All the question is added into the survey";
                return RedirectToAction(nameof(Index));
            }

            List<ViewSurveyQuestionVM> sq = new List<ViewSurveyQuestionVM>()
            {
                new ViewSurveyQuestionVM(){
                    survey_id = survey_id,
                    questionSet = questionsNotInSurvey,
                     questionSetType = questionTypesNotInSurvey,
                     survey_name = await _db.Survey.Where(s=>s.survey_id==survey_id).Select(s=>s.survey_name).FirstOrDefaultAsync()
                }

            };
            return View(sq);


        }
        public async Task<string> GenerateSurveyID()
        {
            string newId;
            string prefix = "S";

            var survey = await _db.Survey
             .OrderByDescending(s => s.survey_id)
             .FirstOrDefaultAsync();

            if (survey != null)
            {
                // Extract the numeric part of the last question ID and increment it by one
                int lastIdNumericPart = int.Parse(survey.survey_id.Substring(1));
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


