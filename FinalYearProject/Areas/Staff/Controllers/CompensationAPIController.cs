using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompensationAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly DbSet<Compensation> dbModel;

        public CompensationAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.Compensation;
        }

        // GET: api/<APIController>
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

        // GET api/<APIController>/5
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

        // POST api/<APIController>
        [HttpPost]
        public async void Post([FromBody] string value)
        {
            dbModel.Add(StringToModel(value));
            await _db.SaveChangesAsync();

        }

        // PUT api/<APIController>/5
        [HttpPut("{id}")]
        public async void Put([FromBody] string value)
        {
            _db.Update(StringToModel(value));
            await _db.SaveChangesAsync();
        }

        // DELETE api/<APIController>/5
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

        private Compensation StringToModel(string value)
        {
            Compensation model = new Compensation();

            string[] infoString = value.Split(",");

            model.comp_id = int.Parse(infoString[0]);
            model.user_id = infoString[1];
            model.comp_type = infoString[2];
            model.comp_desc = infoString[3];
            model.date_applied = DateTime.Parse(infoString[4]);
            model.approved_by = infoString[5];
            model.status = infoString[6];
            model.reject_reason = infoString[7];
            model.date_completed = DateTime.Parse(infoString[8]);

            return model;
        }

        private string ModelToString(Compensation model)
        {
            return
                model.comp_id + "," +
                model.user_id + "," +
                model.comp_type + "," +
                model.comp_desc + "," +
                model.date_applied + "," +
                model.approved_by + "," +
                model.status + "," +
                model.reject_reason + "," +
                model.date_completed
            ;
        }
    }
}
