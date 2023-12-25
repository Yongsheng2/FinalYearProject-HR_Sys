using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Authorization;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = SD.SurveyManage)]
    public class SurveyQuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyQuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SurveyQuestions
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.SurveyQuestion.Include(s => s.Question).Include(s => s.survey);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        public async Task<IActionResult> Index(string survey_id, string company_id, string survey_name, Survey survey)
        {
            var errorMessage = TempData["ErrorMessage2"] as string;

            // Pass the error message to the view
            ViewData["ErrorMessage2"] = errorMessage;

            List<CreateSurveyQuestionVM> surveyList = new List<CreateSurveyQuestionVM>
            {
                new CreateSurveyQuestionVM
                {
                    survey_id = survey_id,
                    company_id = company_id,
                    survey_name = survey_name,
                    questionSet = await _context.Question.Include(q => q.questionType).ToListAsync()

                }
             };

            return View(surveyList);
        }


        // GET: Admin/SurveyQuestions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.SurveyQuestion == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestion
                .Include(s => s.Question)
                .Include(s => s.survey)
                .FirstOrDefaultAsync(m => m.question_id == id);
            if (surveyQuestion == null)
            {
                return NotFound();
            }

            return View(surveyQuestion);
        }


        // GET: Admin/SurveyQuestions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyQuestion surveyQuestion,
            string[] selectedQuestions, string survey_id, string survey_name, string company_id)
        {

            // Initialize the dictionary to store counts for selected questions by question type
            Dictionary<string, int> selectedQuestionTypeCounts = new Dictionary<string, int>();

            // Loop through each selected question
            foreach (var selectedQuestionId in selectedQuestions)
            {
                // Find the corresponding question type for the selected question
                var questionType = await _context.Question
                    .Where(q => q.question_id == selectedQuestionId)
                    .Select(q => q.questionType.questionType_id)
                    .FirstOrDefaultAsync();

                // Check if the question type is already in the dictionary
                if (selectedQuestionTypeCounts.ContainsKey(questionType))
                {
                    // Increment the count if the question type is already in the dictionary
                    selectedQuestionTypeCounts[questionType]++;
                }
                else
                {
                    // Add the question type to the dictionary with a count of 1 if not already present
                    selectedQuestionTypeCounts[questionType] = 1;
                }
            }

            //// Now, update the counts in the provided questionTypeCounts dictionary
            foreach (var entry in selectedQuestionTypeCounts)
            {
                var questionTypeId = entry.Key;
                var count = entry.Value;

                if (count < 3)
                {
                    // Display an error message indicating that the user needs to select at least 3 questions for each type.
                    TempData["ErrorMessage2"] = "Select at least 3 questions for each type.";
                    return RedirectToAction(nameof(Index), new { area = "Staff", controller = "SurveyQuestions", survey_id, survey_name, company_id });
                }
            }

            if (selectedQuestionTypeCounts.Count() == 0)
            {
                TempData["ErrorMessage2"] = "Select at least 3 questions for each type.";
                return RedirectToAction(nameof(Index), new { area = "Staff", controller = "SurveyQuestions", survey_id, survey_name, company_id });
            }

            Survey s = new Survey()
            {
                survey_id = survey_id,
                company_id = company_id,
                survey_name = survey_name,
            };

            _context.Survey.Add(s);

            surveyQuestion.survey_id = survey_id;

            foreach (var questionId in selectedQuestions)
            {
                var surveyQuestionEntry = new SurveyQuestion
                {
                    survey_id = survey_id,
                    question_id = questionId
                };

                _context.SurveyQuestion.Add(surveyQuestionEntry);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewOwnSurveyQuestion), new { area = "Staff", controller = "SurveyQuestions", survey_id });


        }

        public async Task<IActionResult> ViewOwnSurveyQuestion(string survey_id)
        {

            var surveyQuestions = await _context.SurveyQuestion
                .Where(sq => sq.survey_id == survey_id)
                .Include(sq => sq.Question) // find the question in the survey
                .ToListAsync();


            var questions = surveyQuestions.Select(sq => sq.Question).ToList();

            List<ViewSurveyQuestionVM> surveyQuestion = new List<ViewSurveyQuestionVM>()
            {
                new ViewSurveyQuestionVM(){
                    survey_id = survey_id,
                    questionSet = questions,
                     questionSetType = await _context.Question.Include(q => q.questionType).ToListAsync(),
                    survey_name = await _context.Survey.Where(s=>s.survey_id==survey_id).Select(s=>s.survey_name).FirstOrDefaultAsync()
                }

            };
            // Pass the questions to your view
            return View(surveyQuestion);
        }

        // GET: Admin/SurveyQuestions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SurveyQuestion == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestion.FindAsync(id);
            if (surveyQuestion == null)
            {
                return NotFound();
            }
            ViewData["question_id"] = new SelectList(_context.Question, "question_id", "question_id", surveyQuestion.question_id);
            ViewData["survey_id"] = new SelectList(_context.Survey, "survey_id", "survey_id", surveyQuestion.survey_id);
            return View(surveyQuestion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("survey_id,question_id")] SurveyQuestion surveyQuestion)
        {
            if (id != surveyQuestion.question_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surveyQuestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyQuestionExists(surveyQuestion.question_id))
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
            ViewData["question_id"] = new SelectList(_context.Question, "question_id", "question_id", surveyQuestion.question_id);
            ViewData["survey_id"] = new SelectList(_context.Survey, "survey_id", "survey_id", surveyQuestion.survey_id);
            return View(surveyQuestion);
        }

        // GET: Admin/SurveyQuestions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.SurveyQuestion == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestion
                .Include(s => s.Question)
                .Include(s => s.survey)
                .FirstOrDefaultAsync(m => m.question_id == id);
            if (surveyQuestion == null)
            {
                return NotFound();
            }

            return View(surveyQuestion);
        }

        // POST: Admin/SurveyQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SurveyQuestion == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SurveyQuestion'  is null.");
            }
            var surveyQuestion = await _context.SurveyQuestion.FindAsync(id);
            if (surveyQuestion != null)
            {
                _context.SurveyQuestion.Remove(surveyQuestion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyQuestionExists(string id)
        {
            return (_context.SurveyQuestion?.Any(e => e.question_id == id)).GetValueOrDefault();
        }
    }
}
