using FinalYearProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Areas.Staff.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DateTimeAPIController : ControllerBase
    {
        [HttpGet]
        public DateTime Get()
        {
            return DateTime.Now;
        }
    }
}
