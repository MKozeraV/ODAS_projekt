using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjektODASAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTController : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("GetData")]
        public string GetData()
        {
            return "Authemticated with JWT";
        }

        [HttpGet]
        [Route("Details")]
        public string Details()
        {
            return "Authemticated with JWT";
        }



    }
}
