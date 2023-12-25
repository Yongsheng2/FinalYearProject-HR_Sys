using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly DbSet<Attendance> dbModel;

        public AttendanceAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.Attendance;
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

        private Attendance StringToModel(string value)
        {
            Attendance model = new Attendance();

            string[] infoString = value.Split(",");
            int i = -1;

            model.attendance_id = infoString[i++];
            model.staff_id = infoString[i++];
            model.shift_id = infoString[i++];
            model.start_time = DateTime.Parse(infoString[i++]);
            model.end_time = DateTime.Parse(infoString[i++]);
            model.validity = bool.Parse(infoString[i++]);
            model.on_leave = bool.Parse(infoString[i++]);

            return model;
        }

        private string ModelToString(Attendance model)
        {
            return
                model.attendance_id + "," +
                model.staff_id + "," +
                model.shift_id + "," +
                model.start_time + "," +
                model.end_time + "," +
                model.validity + "," +
                model.on_leave
            ;
        }
    }
}
