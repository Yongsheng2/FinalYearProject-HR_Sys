using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<EmployeeClaim> dbModel;

        public ClaimAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.EmployeeClaim;
        }

        // GET: api/<ClaimAPIController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var queryList = await dbModel.ToListAsync();

            List<string> resultList = new List<string>();

            foreach (var item in queryList)
            {
                resultList.Add(ModelToString(item));
            }

            string[] result = resultList.ToArray();

            return result;
        }

        // GET api/<ClaimAPIController>/5
        [HttpGet("{id}")]
        public async Task<string> Get([FromBody] string value)
        {
            var item = await dbModel.FindAsync(value);
            string result;

            if (item == null)
            {
                result = "Not Found 404";
            }
            else
            {
                result = ModelToString(item);
            }

            return result;
        }

        // POST api/<ClaimAPIController>
        [HttpPost]
        public async void Post([FromBody] string value)
        {
            dbModel.Add(StringToModel(value));
            await _db.SaveChangesAsync();
        }

        // PUT api/<ClaimAPIController>/5
        [HttpPut("{id}")]
        public async void Put([FromBody] string value)
        {
            _db.Update(StringToModel(value));
            await _db.SaveChangesAsync();
        }

        // DELETE api/<ClaimAPIController>/5
        [HttpDelete("{id}")]
        public async Task<string> Delete(string id)
        {
            var model = await dbModel.FindAsync(id);
            if (model == null)
            {
                return "Not Found 404";
            }
            else
            {
                dbModel.Remove(model);
                await _db.SaveChangesAsync();
                return id + "Delete Success";
            }
        }

        private EmployeeClaim StringToModel(string value)
        {
            EmployeeClaim model = new EmployeeClaim();

            string[] infoString = value.Split(",");

            model.claim_id = infoString[0];
            model.staff_id = infoString[1];
            model.approval_status = infoString[2];
            model.date_apply = DateTime.Now;
            model.claim_reason = infoString[6];
            model.claimAmount = infoString[7];
            model.date_apply = DateTime.Parse(infoString[8]);

            return model;
        }

        private string ModelToString(EmployeeClaim model)
        {
            return
                model.claim_id + "," +
                model.staff_id + "," +
                model.approval_status + "," +
                model.claim_reason + "," +
                model.claimAmount + "," +
                model.date_apply + ","
            ;
        }
    }
}