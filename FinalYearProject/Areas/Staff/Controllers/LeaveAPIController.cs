using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly DbSet<Leave> dbModel;

        public LeaveAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.Leave;
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

        private Leave StringToModel(string value)
        {
            Leave model = new Leave();

            string[] infoString = value.Split(",");

            model.leave_id = infoString[0];
            model.staff_id = infoString[1];
            model.approval_status = infoString[2];
            model.approved_by = infoString[3];
            model.date_created = DateTime.Now;
            model.leave_start = DateTime.Parse(infoString[4]);
            model.leave_end = DateTime.Parse(infoString[5]);
            model.leave_reason = infoString[6];
            model.response_message = infoString[7];

            return model;
        }

        private string ModelToString(Leave model)
        {
            return 
                model.leave_id + "," + 
                model.staff_id + "," + 
                model.approval_status + "," +
                model.approved_by + "," +
                model.leave_start + "," +
                model.leave_end + "," +
                model.leave_reason + "," +
                model.response_message
            ;
        }
    }
}
