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
using Microsoft.Data.SqlClient;


namespace FinalYearProject.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class SurveyTakensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyTakensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff/SurveyTakens
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SurveyTaken.Include(s => s.EmployeeDetails).Include(s => s.Survey);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Staff/SurveyTakens/Details/5
        public async Task<IActionResult> Details(string id, decimal total_score,string surveyId)
        {

            if (total_score == 0.0M) {
                var surveyTaken = await _context.SurveyTaken
             .Where(st => st.surveyTaken_id == id)
             .FirstOrDefaultAsync();

                if (surveyTaken != null)
                {
                    total_score = (decimal)surveyTaken.total_score;
                    surveyId = surveyTaken.survey_id;
                }

            }

            var survey = await _context.SurveyTaken.Where(s=>s.surveyTaken_id==id).FirstOrDefaultAsync();

            var existingQuestionIds = await _context.Answer
            .Where(a => a.surveyTaken_id == id)
            .Select(sq => sq.question_id)
            .ToListAsync();

            var questionsInSurvey = await _context.Question
            .Where(q => existingQuestionIds.Contains(q.question_id))
            .ToListAsync();

            var answerSet = await _context.Answer
            .Where(a => a.surveyTaken_id == id)
            .ToListAsync();


            List<StaffAnswerSurveyVM> sas = new List<StaffAnswerSurveyVM>()
            {
                new StaffAnswerSurveyVM(){
                    surveyTaken_id = id,
                    questionSet = questionsInSurvey,
                    answersSet = answerSet,
                    total_score=total_score,
                    survey_id = surveyId,
                    questionSetType = await _context.Question.Include(q => q.questionType).ToListAsync(),
                    survey_name = await _context.Survey.Where(s=>s.survey_id == survey.survey_id).Select(s => s.survey_name).FirstOrDefaultAsync(),
                }
            };
            return View(sas);
        }

        public IActionResult Create()
        {
            ViewData["employee_id"] = new SelectList(_context.EmployeeDetails, "employee_id", "employee_id");
            ViewData["survey_id"] = new SelectList(_context.Survey, "survey_id", "survey_id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("surveyTaken_id,employee_id,survey_id")] SurveyTaken surveyTaken)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surveyTaken);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["employee_id"] = new SelectList(_context.EmployeeDetails, "employee_id", "employee_id", surveyTaken.employee_id);
            ViewData["survey_id"] = new SelectList(_context.Survey, "survey_id", "survey_id", surveyTaken.survey_id);
            return View(surveyTaken);
        }

        public async Task<IActionResult> Edit(string id)
        {

            decimal total_score = (decimal)await _context.SurveyTaken.Where(st => st.surveyTaken_id == id).Select(st => st.total_score).FirstOrDefaultAsync();

            var survey = await _context.SurveyTaken.Where(st => st.surveyTaken_id == id).FirstOrDefaultAsync();


            var existingQuestionIds = await _context.Answer
            .Where(a => a.surveyTaken_id == id)
            .Select(sq => sq.question_id)
            .ToListAsync();

            var questionsInSurvey = await _context.Question
            .Where(q => existingQuestionIds.Contains(q.question_id))
            .ToListAsync();

            var answerSet = await _context.Answer
            .Where(a => a.surveyTaken_id == id)
            .ToListAsync();


            List<StaffAnswerSurveyVM> sas = new List<StaffAnswerSurveyVM>()
            {
                new StaffAnswerSurveyVM(){
                    surveyTaken_id = id,
                    questionSet = questionsInSurvey,
                    answersSet = answerSet,
                    total_score=total_score,
                    survey_id = survey.survey_id,
                    questionSetType = await _context.Question.Include(q => q.questionType).ToListAsync(),
                    survey_name = await _context.Survey.Where(s=>s.survey_id == survey.survey_id).Select(s => s.survey_name).FirstOrDefaultAsync(),

                }
            };
            return View(sas);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string selectedSurveyId, Dictionary<string, int> answers, string[] question_id, string surveyTakenID)
        {
            var aspId = User.Identity?.Name;
            var currentUser = await _context.EmployeeDetails.Where(e => e.user_id == aspId).FirstOrDefaultAsync();
            decimal totalScore = 0;




            SurveyTaken st = new SurveyTaken()
            {
                surveyTaken_id = surveyTakenID,
                survey_id = selectedSurveyId,
                employee_id = currentUser.employee_id
            };

            
            // Add answers
            foreach (var entry in answers)
            {
                string questionId = entry.Key; // This is the question ID
                int answer = entry.Value;      // This is the selected answer
                                               // Do something with questionId and answer               

                Answer ans = new Answer()
                {
                    answer_id = await _context.Answer.Where(a=> a.surveyTaken_id==st.surveyTaken_id && a.question_id==questionId).Select(a=>a.answer_id).FirstOrDefaultAsync(),
                    surveyTaken_id = st.surveyTaken_id,
                    question_id = questionId,
                    answer = answer
                };
                totalScore += answer;

                _context.Answer.Update(ans);
                await _context.SaveChangesAsync();
            }

            // Calculate the total score
            decimal sumScore = await _context.Answer
                .Where(a => a.surveyTaken_id == st.surveyTaken_id)
                .SumAsync(a => (decimal?)a.answer) ?? 0;

            // Calculate the total number of answers
            int totalAnswers = await _context.Answer
                .Where(a => a.surveyTaken_id == st.surveyTaken_id)
                .CountAsync();

            // Perform the division
            totalScore = totalAnswers > 0 ? sumScore / totalAnswers : 0;

            // Now 'averageScore' contains the result of dividing the total score by the total number of answers.



            SurveyTaken existingSurveyTaken = await _context.SurveyTaken
                .FirstOrDefaultAsync(stIndb => stIndb.surveyTaken_id == st.surveyTaken_id);

            // Update total_score property
            if (existingSurveyTaken != null)
            {
                existingSurveyTaken.total_score = totalScore;

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { area = "Staff", controller = "SurveyTakens", id = surveyTakenID, total_score = totalScore, surveyId = st.survey_id });


        }

        public async Task<IActionResult> Delete(string id)
        {
            decimal total_score = (decimal)await _context.SurveyTaken.Where(st => st.surveyTaken_id == id).Select(st => st.total_score).FirstOrDefaultAsync();

            var survey = await _context.SurveyTaken.Where(st => st.surveyTaken_id == id).FirstOrDefaultAsync();


            var existingQuestionIds = await _context.Answer
            .Where(a => a.surveyTaken_id == id)
            .Select(sq => sq.question_id)
            .ToListAsync();

            var questionsInSurvey = await _context.Question
            .Where(q => existingQuestionIds.Contains(q.question_id))
            .ToListAsync();

            var answerSet = await _context.Answer
            .Where(a => a.surveyTaken_id == id)
            .ToListAsync();


            List<StaffAnswerSurveyVM> sas = new List<StaffAnswerSurveyVM>()
            {
                new StaffAnswerSurveyVM(){
                    surveyTaken_id = id,
                    questionSet = questionsInSurvey,
                    answersSet = answerSet,
                    total_score=total_score,
                    survey_id = survey.survey_id,
                    questionSetType = await _context.Question.Include(q => q.questionType).ToListAsync(),
                     survey_name = await _context.Survey.Where(s=>s.survey_id == survey.survey_id).Select(s => s.survey_name).FirstOrDefaultAsync()
                }

                
            };
            return View(sas);
        }

        // POST: Staff/SurveyTakens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SurveyTaken == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SurveyTaken'  is null.");
            }

            var answerToDelete = await _context.Answer
               .Where(ans => ans.surveyTaken_id == id)
               .ToListAsync();

            if (answerToDelete != null)
            {
                // Remove the found entries from the DbSet
                _context.Answer.RemoveRange(answerToDelete);
            }

            var surveyTaken = await _context.SurveyTaken.FindAsync(id);

            if (surveyTaken != null)
            {
                _context.SurveyTaken.Remove(surveyTaken);

            }
           


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyTakenExists(string id)
        {
          return (_context.SurveyTaken?.Any(e => e.surveyTaken_id == id)).GetValueOrDefault();
        }

        public async Task<string> GenerateSurveyTakenID()
        {
            string newId;
            string prefix = "ST";

            // Retrieve the last question ID from the database
            var surveyTaken = await _context.SurveyTaken
                .OrderByDescending(s => s.surveyTaken_id)
                .FirstOrDefaultAsync();

            if (surveyTaken != null)
            {
                // Extract the numeric part of the last question ID and increment it by one
                int lastIdNumericPart = int.Parse(surveyTaken.surveyTaken_id.Substring(2));
                newId = prefix + (lastIdNumericPart + 1).ToString("00000");
            }
            else
            {
                // If no question exists, start with Q00001
                newId = prefix + "00001";
            }

            return newId;
        }

        public async Task<String> GenerateAnswerID()
        {
            string newId;
            string prefix = "A";

            // Retrieve the last question ID from the database
            var ans = await _context.Answer
                .OrderByDescending(a => a.answer_id)
                .FirstOrDefaultAsync();

            if (ans != null)
            {
                // Extract the numeric part of the last question ID and increment it by one
                int lastIdNumericPart = int.Parse(ans.answer_id.Substring(1));
                newId = prefix + (lastIdNumericPart + 1).ToString("00000");
            }
            else
            {
                // If no question exists, start with Q00001
                newId = prefix + "00001";
            }

            return newId;
        }
        public async Task<IActionResult> ViewCompanySurvey()
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
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    var allSurvey = await _context.Survey
                        .Where(s => s.company_id == currentUser.parent_company).Select(s => s.survey_id).ToListAsync();

                    var surveyNoTaken = await _context.SurveyTaken                      
                        .Where(st => st.employee_id == currentUser.employee_id)
                        .Select(st => st.survey_id).ToListAsync();

                    var surveyLeft = await _context.Survey
                        .Where(sl => !surveyNoTaken.Contains(sl.survey_id))
                         .Where(s => s.company_id == currentUser.parent_company).ToListAsync();

                    return View(surveyLeft);

                }
            }
        }

        public async Task<IActionResult> AnswerSurvey(string selectedSurveyId) {

            var existingQuestionIds = await _context.SurveyQuestion
                .Where(sq => sq.survey_id == selectedSurveyId)
                .Select(sq => sq.question_id)
                .ToListAsync();

            var questionsInSurvey = await _context.Question
                .Where(q => existingQuestionIds.Contains(q.question_id))
                .ToListAsync();

            List<StaffAnswerSurveyVM> sq = new List<StaffAnswerSurveyVM>
            {
                new StaffAnswerSurveyVM {
                surveyTaken_id = GenerateSurveyTakenID().Result,
                survey_id = selectedSurveyId,
                questionSet = questionsInSurvey,
                questionSetType = await _context.Question.Include(q => q.questionType).ToListAsync(),
                survey_name = await _context.Survey.Where(s=>s.survey_id == selectedSurveyId).Select(s => s.survey_name).FirstOrDefaultAsync(),
                }
            };

            return View(sq);
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnswerSurvey(string selectedSurveyId, Dictionary<string, int> answers, string[] question_id, string surveyTakenID)
        {
            var aspId = User.Identity?.Name;
            var currentUser = await _context.EmployeeDetails.Where(e => e.user_id == aspId).FirstOrDefaultAsync();
            decimal totalScore = 0;


  

            SurveyTaken st = new SurveyTaken()
            {
                surveyTaken_id = GenerateSurveyTakenID().Result,
                survey_id = selectedSurveyId,
                employee_id = currentUser.employee_id
            };

            _context.SurveyTaken.Add(st);
            await _context.SaveChangesAsync();

            // Add answers
            foreach (var entry in answers)
            {
                string questionId = entry.Key; // This is the question ID
                int answer = entry.Value;      // This is the selected answer
                                               // Do something with questionId and answer                       
                Answer ans = new Answer()
                {
                    answer_id = GenerateAnswerID().Result,
                    surveyTaken_id = st.surveyTaken_id, 
                    question_id = questionId,
                    answer = answer
                };               
                    totalScore += answer;                                

                _context.Answer.Add(ans);
                await _context.SaveChangesAsync();
            }

            // Calculate the total score
            decimal sumScore = await _context.Answer
                .Where(a => a.surveyTaken_id == st.surveyTaken_id)
                .SumAsync(a => (decimal?)a.answer) ?? 0;

            // Calculate the total number of answers
            int totalAnswers = await _context.Answer
                .Where(a => a.surveyTaken_id == st.surveyTaken_id)
                .CountAsync();

            // Perform the division
             totalScore = totalAnswers > 0 ? sumScore / totalAnswers : 0;

            // Now 'averageScore' contains the result of dividing the total score by the total number of answers.



            SurveyTaken existingSurveyTaken = await _context.SurveyTaken
                .FirstOrDefaultAsync(stIndb => stIndb.surveyTaken_id == st.surveyTaken_id);

            // Update total_score property
            if (existingSurveyTaken != null)
            {
                existingSurveyTaken.total_score = totalScore;

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { area = "Staff", controller = "SurveyTakens", id = surveyTakenID, total_score=totalScore, surveyId=st.survey_id });


        }


        public async Task<IActionResult> PredictSatisfaction(string surveyTakenID, decimal totalScore)
     
        {
            var surveyTaken = await _context.SurveyTaken.FindAsync(surveyTakenID);

            if (surveyTaken == null)
            {
                // Handle the case when the surveyTaken is not found

                return NotFound();
            }

            // Load sample data
            var sampleData = new MLModel3.ModelInput()
            {
                Total_score = (float)totalScore,
            };

            // Load model and predict output
            var result = MLModel3.Predict(sampleData);

            // Pass the result to the view
            ViewBag.PredictionResult = result;

            if (result.PredictedLabel == "FALSE") {
                await PredictUnsatisfyReason(surveyTakenID);
            }

            return View();
        }

        public async Task<IActionResult> PredictUnsatisfyReason(string surveyTakenID)
        {
            var surveyTaken = await _context.SurveyTaken.FindAsync(surveyTakenID);

            var surveyResults = await (
                from qt in _context.QuestionType
                join q in _context.Question on qt.questionType_id equals q.questionType_id
                join sq in _context.SurveyQuestion on q.question_id equals sq.question_id
                join st in _context.SurveyTaken on sq.survey_id equals st.survey_id
                join a in _context.Answer on q.question_id equals a.question_id
                where st.surveyTaken_id == surveyTakenID
                group new { qt, st, a } by new { qt.questionType_id, st.surveyTaken_id, st.survey_id } into grouped
                select new
                {
                    grouped.Key.questionType_id,
                    grouped.Key.surveyTaken_id,
                    grouped.Key.survey_id,
                    AverageScore = Math.Round(grouped.Sum(x => (decimal)x.a.answer) / grouped.Select(x => x.a.question_id).Distinct().Count(), 2)
                }
            ).ToListAsync();

            var averageScores = surveyResults.ToDictionary(result => result.questionType_id, result => result.AverageScore);

            float companyCultureScore = 0.0F;
            float jobSatisfactionScore = 0.0F; 
            float professionalGrowthScore = 0.0F; 
            float managerRelationshipScore = 0.0F; 
            float compensationBenefitsScore = 0.0F; 
            float workLifeBalanceScore = 0.0F;

            var allQuestionTypeIds = await _context.QuestionType.Select(qt => qt.questionType_id).ToListAsync();
            var questionTypeScores = new Dictionary<string, float>();

            foreach (var questionTypeId in allQuestionTypeIds)
            {
                float questionTypeScore = 0.0F;

                if (averageScores.TryGetValue(questionTypeId, out var averageScore))
                {
                    questionTypeScore = (float)averageScore;
                }

                questionTypeScores.Add(questionTypeId, questionTypeScore);
            }

            
            foreach (var questionTypeId in questionTypeScores.Keys)
            {
                if (questionTypeId == "QT00001")
                {
                    companyCultureScore = questionTypeScores[questionTypeId];
                }
                else if (questionTypeId == "QT00002")
                {
                    jobSatisfactionScore = questionTypeScores[questionTypeId];
                }
                else if (questionTypeId == "QT00003")
                {
                    professionalGrowthScore = questionTypeScores[questionTypeId];
                }
                else if (questionTypeId == "QT00004")
                {
                    managerRelationshipScore = questionTypeScores[questionTypeId];
                }
                else if (questionTypeId == "QT00005")
                {
                    compensationBenefitsScore = questionTypeScores[questionTypeId];
                }
                else if (questionTypeId == "QT00006")
                {
                    workLifeBalanceScore = questionTypeScores[questionTypeId];
                }
            }



            //Load sample data
            var sampleData = new MLModel4.ModelInput()
            {
                Company_culture = companyCultureScore, 
                Job_satisfaction = jobSatisfactionScore,
                Professional_growth = professionalGrowthScore,
                Manager_relationship = managerRelationshipScore,
                Compensation_and_benefits = compensationBenefitsScore,
                Work_life_balance = workLifeBalanceScore
            };

            var result = MLModel4.Predict(sampleData);
            
            int position=0;

            
            ViewBag.PredictionResultReason = result;

            var questionTypeNames = _context.QuestionType.Select(qt => qt.questionType_name).ToArray();
            for (int i = 0; i < questionTypeNames.Length; i++)
            {
                if (questionTypeNames[i].Contains(result.PredictedLabel))
                {
                    position = i;
                    break; 
                }
            }

            ViewBag.positionOfReason = position;

            var predictQuestionTypeScores = new Dictionary<string, float>();

            // Populate the dictionary with question type names and scores
            for (int i = 0; i < questionTypeNames.Length; i++)
            {
                var questionTypeName = questionTypeNames[i];
                var score = result.Score[i];
                predictQuestionTypeScores[questionTypeName] = score*100;
            }

            ViewBag.QuestionTypeScores = predictQuestionTypeScores;
            return View();
        }


    }


}
