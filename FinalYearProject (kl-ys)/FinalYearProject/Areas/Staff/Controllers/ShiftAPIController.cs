using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly DbSet<Shift> dbModel;

        public ShiftAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.Shift;
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

        private Shift StringToModel(string value)
        {
            Shift model = new Shift();

            string[] infoString = value.Split(",");

            model.shift_id = infoString[0];
            model.shift_start = DateTime.Parse(infoString[1]);
            model.shift_end = DateTime.Parse(infoString[2]);
            model.is_overtime = bool.Parse(infoString[3]);
            model.parent_shift = infoString[4];
            model.qr_code = infoString[5];
            model.payrate_id = infoString[6];

            return model;
        }

        private string ModelToString(Shift model)
        {
            return
                model.shift_id + "," +
                model.shift_start + "," +
                model.shift_end + "," +
                model.is_overtime + "," +
                model.parent_shift + "," +
                model.qr_code + "," +
                model.payrate_id 
            ;
        }
    }
}
