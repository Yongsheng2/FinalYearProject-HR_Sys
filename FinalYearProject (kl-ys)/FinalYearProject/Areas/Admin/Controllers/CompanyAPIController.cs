using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FinalYearProject.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly DbSet<Company> dbModel;

        public CompanyAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.Company;
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

        private Company StringToModel(string value)
        {
            Company model = new Company();

            string[] infoString = value.Split(",");

            model.company_id = infoString[0];
            model.company_name = infoString[1];
            model.date_created = DateTime.Now;
            model.current_admin = infoString[3];
            model.address = infoString[4];
            model.longitude = infoString[5];
            model.latitude = infoString[6];

            return model;
        }

        private string ModelToString(Company model)
        {
            return 
                model.company_id + ","  + 
                model.company_name + "," + 
                model.current_admin + "," +
                model.address + "," +
                model.longitude + "," +
                model.latitude
                ;
        }
    }
}
