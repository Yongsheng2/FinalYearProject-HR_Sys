using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FinalYearProject.Utility;
using Microsoft.AspNetCore.Authorization;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = SD.SurveyManage)]
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _db;

        public QuestionController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            // Retrieve the error message from TempData
            var errorMessage = TempData["ErrorMessage"] as string;

            // Pass the error message to the view
            ViewData["ErrorMessage"] = errorMessage;

            var questionTypeIds = await _db.Question.Select(q => q.questionType_id).ToListAsync();

            // Retrieve questionType names from the client side
            var listQuestionTypeNames = await _db.QuestionType
                .Where(qt => questionTypeIds.Contains(qt.questionType_id))
                .ToListAsync();

            // Create a dictionary to associate questionType names with questionType_ids
            //var dictionary = listQuestionTypeNames.ToDictionary(qt => qt.questionType_id, qt => qt.questionType_name);

            //// Use the dictionary to get the names in the correct order
            //var flattenedList = questionTypeIds.Select(id => dictionary.GetValueOrDefault(id)).ToList();

            List<ViewQuestionVM> cq = new List<ViewQuestionVM>
            {
                new ViewQuestionVM()
                {
                    question = await _db.Question.ToListAsync(),
                    questionSetType = await _db.Question.Include(q => q.questionType).ToListAsync()
                }

        };


            return View(cq);
        }

        public async Task<IActionResult> Create()
        {
            // Retrieve the error message from TempData
            var errorMessage = TempData["ErrorMessageForQuestionType"] as string;

            // Pass the error message to the view
            ViewData["ErrorMessageForQuestionType"] = errorMessage;


            Question questions = new Question()
            {
                question_id = GenerateQuestionID().Result,

            };

            List<CreateQuestionVM> cq = new List<CreateQuestionVM>
            {
                new CreateQuestionVM
                {
                     question = questions,
                     questionTypeSet = await _db.QuestionType.ToListAsync()

                }

            };
            return View(cq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateQuestionVM qt, string questionTypeID, string questionTypeName)
        {
            if (questionTypeID == null)
            {
                TempData["ErrorMessageForQuestionType"] = "Please select one question type for your question.";
                return RedirectToAction(nameof(Create));
            }
            if (qt.question == null)
            {
                TempData["ErrorMessageForQuestion"] = "Please enter question.";
                return RedirectToAction(nameof(Create));
            }
            Question q = new Question()
            {
                question = qt.question.question,
                question_id = qt.question.question_id,
                questionType_id = questionTypeID

            };


            _db.Question.Add(q);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _db.Question.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            var admins = await _db.Admin.ToListAsync();
            ViewBag.current_admin = new SelectList(admins.AsEnumerable(), "admin_id", "admin_id");

            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Question q)
        {
            if (ModelState.IsValid)
            {
                _db.Update(q);
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

            var question = await _db.Question.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            var question = await _db.Question.FindAsync(id);

            if (question == null)
            {
                return View();
            }

            // Check if the question is associated with any surveys
            bool isQuestionUsedInSurveys = await _db.SurveyQuestion.AnyAsync(sq => sq.question_id == id);

            if (isQuestionUsedInSurveys)
            {
                // If the question is in any surveys
                TempData["ErrorMessage"] = "Cannot delete the question because it is used in surveys.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _db.Question.Remove(question);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }



        public async Task<string> GenerateQuestionID()
        {
            string newId;
            string prefix = "Q";

            // Retrieve the last question ID from the database
            var lastQuestion = await _db.Question
                .OrderByDescending(q => q.question_id)
                .FirstOrDefaultAsync();

            if (lastQuestion != null)
            {
                // Extract the numeric part of the last question ID and increment it by one
                int lastIdNumericPart = int.Parse(lastQuestion.question_id.Substring(1));
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
