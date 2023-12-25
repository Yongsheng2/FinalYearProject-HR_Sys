using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly DbSet<EmployeeDetails> dbModel;

        public EmployeeAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.EmployeeDetails;
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

        private EmployeeDetails StringToModel(string value)
        {
            EmployeeDetails model = new EmployeeDetails();

            string[] infoString = value.Split(",");
            int i = -1;

            model.employee_id = infoString[i++];
            model.employee_id_by_company = infoString[i++];
            model.employee_name = infoString[i++];
            model.user_id = infoString[i++];
            model.parent_company = infoString[i++];
            model.staff_role = infoString[i++];
            model.acc_pass = infoString[i++];
            model.employer_id = infoString[i++];
            model.employment_start_date = DateTime.Parse(infoString[i++]);
            model.types_of_wages = infoString[i++];
            model.wages_rate = float.Parse(infoString[i++]);
            model.employement_letter = bool.Parse(infoString[i++]);
            model.monthly_deduction = float.Parse(infoString[i++]);
            model.ic_no = infoString[i++];
            model.dob = DateTime.Parse(infoString[i++]);
            model.gender = infoString[i++];
            model.nationality = infoString[i++];
            model.phone_no = infoString[i++];
            model.email = infoString[i++];
            model.epf_no = infoString[i++];
            model.sosco_no = infoString[i++];
            model.itax_no = infoString[i++];
            model.bank_name = infoString[i++];
            model.bank_no = infoString[i++];
            model.religion = infoString[i++];
            model.paidLeaveHourLeft = int.Parse(infoString[i++]);
            model.paidLeaveOnBargain = int.Parse(infoString[i++]);
            model.sickLeaveHourLeft = int.Parse(infoString[i++]);
            model.sickLeaveOnBargain = int.Parse(infoString[i++]);
            model.uuid = infoString[i++];
            model.leaveUpdate = DateTime.Parse(infoString[i++]);

            return model;
        }

        private string ModelToString(EmployeeDetails model)
        {
            return
                model.employee_id + "," +
                model.employee_id_by_company + "," +
                model.user_id + "," +
                model.parent_company + "," +
                model.staff_role + "," +
                model.acc_pass + "," +
                model.employer_id + "," +
                model.employment_start_date + "," +
                model.types_of_wages + "," +
                model.wages_rate + "," +
                model.employement_letter + "," +
                model.monthly_deduction + "," +
                model.ic_no + "," +
                model.dob + "," +
                model.gender + "," +
                model.nationality + "," +
                model.phone_no + "," +
                model.email + "," +
                model.epf_no + "," +
                model.sosco_no + "," +
                model.itax_no + "," +
                model.bank_name + "," +
                model.bank_no + "," +
                model.religion + "," +
                model.paidLeaveHourLeft + "," +
                model.paidLeaveOnBargain + "," +
                model.sickLeaveHourLeft + "," +
                model.sickLeaveOnBargain + "," +
                model.uuid + "," +
                model.leaveUpdate
            ;
        }
    }
}
