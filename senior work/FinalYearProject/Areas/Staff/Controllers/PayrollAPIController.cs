using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly DbSet<Models.Payroll> dbModel;

        public PayrollAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.Payroll;
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

        private Models.Payroll StringToModel(string value)
        {
            Models.Payroll model = new Models.Payroll();

            string[] infoString = value.Split(",");

            model.payroll_id = infoString[0];
            model.staff_id = infoString[1];
            model.month_salary = float.Parse(infoString[2]);
            model.overtime_pay = float.Parse(infoString[3]);
            model.kwsp_total = float.Parse(infoString[4]);
            model.zakat_total = float.Parse(infoString[5]);

            return model;
        }

        private string ModelToString(Models.Payroll model)
        {
            return
                model.payroll_id + "," +
                model.staff_id + "," +
                model.month_salary + "," +
                model.overtime_pay + "," +
                model.kwsp_total + "," +
                model.zakat_total 
            ;
        }
    }
}
