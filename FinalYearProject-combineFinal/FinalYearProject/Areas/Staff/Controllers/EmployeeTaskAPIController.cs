using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeTaskAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<EmployeeTask> dbModel;

        public EmployeeTaskAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.EmployeeTasks;
        }

        // GET: api/<EmployeeTaskAPIController>
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

        // GET api/<EmployeeTaskAPIController>/5
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

        // POST api/<EmployeeTaskAPIController>
        [HttpPost]
        public async void Post([FromBody] string value)
        {
            dbModel.Add(StringToTaskModel(value));
            await _db.SaveChangesAsync();
        }

        // PUT api/<EmployeeTaskAPIController>/5
        [HttpPut("{id}")]
        public async void Put([FromBody] string value)
        {
            _db.Update(StringToTaskModel(value));
            await _db.SaveChangesAsync();
        }

        // DELETE api/<EmployeeTaskAPIController>/5
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

        private EmployeeTask StringToTaskModel(string value)
        {
            EmployeeTask model = new EmployeeTask();

            string[] infoString = value.Split(",");

            model.emtask_id = infoString[0];
            model.emtask_title = infoString[1];
            model.current_admin = infoString[2];
            model.staff_id = infoString[3];
            model.progress_status = infoString[4];
            model.emtask_duration = int.Parse(infoString[5]);
            model.emtaskDetail = infoString[4];
            model.emtaskdoneFile = infoString[5];
            model.date_upload = DateTime.Parse(infoString[8]);

            return model;
        }

        private string ModelToString(EmployeeTask model)
        {
            return
                model.emtask_id + "," +
                model.emtask_title + "," +
                model.current_admin + "," +
                model.staff_id + "," +
                model.progress_status + "," +
                model.emtask_duration + "," +
                model.emtaskDetail + "," +
                model.emtaskdoneFile + "," +
                model.date_upload + ","
            ;
        }
    }
}
