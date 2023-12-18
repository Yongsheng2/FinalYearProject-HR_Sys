using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Areas.Staff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatboxAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<Chatbox> dbModel;

        public ChatboxAPIController(ApplicationDbContext db)
        {
            _db = db;
            dbModel = _db.Chatboxs;
        }

        // GET: api/<ChatboxAPIController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var queryList = await dbModel.ToListAsync();

            List<string> resultList = new List<string>();

            foreach (var item in queryList)
            {
                resultList.Add(ModelToString(item));
            }

            return resultList.ToArray();
        }

        // GET api/<ChatboxAPIController>/5
        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            var item = await dbModel.FindAsync(id);
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

        // POST api/<ChatboxAPIController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            dbModel.Add(StringToModel(value));
            await _db.SaveChangesAsync();
            return Ok("Chatbox added successfully");
        }

        // PUT api/<ChatboxAPIController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] string value)
        {
            var existingChatbox = await dbModel.FindAsync(id);

            if (existingChatbox == null)
            {
                return NotFound("Chatbox not found");
            }

            var updatedChatbox = StringToModel(value);
            updatedChatbox.chat_id = existingChatbox.chat_id;
            _db.Entry(existingChatbox).CurrentValues.SetValues(updatedChatbox);

            await _db.SaveChangesAsync();

            return Ok("Chatbox updated successfully");
        }

        // DELETE api/<ChatboxAPIController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await dbModel.FindAsync(id);
            if (model == null)
            {
                return NotFound("Chatbox not found");
            }
            else
            {
                dbModel.Remove(model);
                await _db.SaveChangesAsync();
                return Ok(id + " Delete Success");
            }
        }

        private Chatbox StringToModel(string value)
        {
            Chatbox model = new Chatbox();

            string[] infoString = value.Split(",");

            model.chat_id = infoString[0];
            model.staff_id = infoString[1];
            model.admin_id = infoString[2];
            model.send_id = infoString[3];
            model.send_name = infoString[4];
            model.receive_id = infoString[5];
            model.receive_name = infoString[6]; 
            model.chat_ctn = infoString[7];
            model.timestamp = DateTime.Parse(infoString[8]);

            return model;
        }

        private string ModelToString(Chatbox model)
        {
            return
                model.chat_id + "," +
                model.staff_id + "," +
                model.admin_id + "," +
                model.send_id + "," +
                model.send_name + "," +  
                model.receive_id + "," +
                model.receive_name + "," + 
                model.chat_ctn + "," +
                model.timestamp + ",";
        }

    }
}
