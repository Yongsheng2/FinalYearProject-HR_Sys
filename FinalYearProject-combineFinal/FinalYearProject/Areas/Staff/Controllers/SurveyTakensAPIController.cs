using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalYearProject.Data;
using FinalYearProject.Models;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyTakensAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SurveyTakensAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SurveyTakens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyTaken>>> GetSurveyTaken()
        {
          if (_context.SurveyTaken == null)
          {
              return NotFound();
          }
            return await _context.SurveyTaken.ToListAsync();
        }

        // GET: api/SurveyTakens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyTaken>> GetSurveyTaken(string id)
        {
          if (_context.SurveyTaken == null)
          {
              return NotFound();
          }
            var surveyTaken = await _context.SurveyTaken.FindAsync(id);

            if (surveyTaken == null)
            {
                return NotFound();
            }

            return surveyTaken;
        }

        // PUT: api/SurveyTakens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurveyTaken(string id, SurveyTaken surveyTaken)
        {
            if (id != surveyTaken.surveyTaken_id)
            {
                return BadRequest();
            }

            _context.Entry(surveyTaken).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyTakenExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SurveyTakens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SurveyTaken>> PostSurveyTaken(SurveyTaken surveyTaken)
        {
          if (_context.SurveyTaken == null)
          {
              return Problem("Entity set 'ApplicationDbContext.SurveyTaken'  is null.");
          }
            _context.SurveyTaken.Add(surveyTaken);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SurveyTakenExists(surveyTaken.surveyTaken_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSurveyTaken", new { id = surveyTaken.surveyTaken_id }, surveyTaken);
        }

        // DELETE: api/SurveyTakens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurveyTaken(string id)
        {
            if (_context.SurveyTaken == null)
            {
                return NotFound();
            }
            var surveyTaken = await _context.SurveyTaken.FindAsync(id);
            if (surveyTaken == null)
            {
                return NotFound();
            }

            _context.SurveyTaken.Remove(surveyTaken);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyTakenExists(string id)
        {
            return (_context.SurveyTaken?.Any(e => e.surveyTaken_id == id)).GetValueOrDefault();
        }
    }
}
