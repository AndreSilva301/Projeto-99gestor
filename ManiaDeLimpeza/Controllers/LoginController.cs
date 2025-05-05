using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Register")]
        public ActionResult<User> RegisterPost()
        {
            throw new NotImplementedException();
        }


        [HttpGet(Name = "Login")]
        public ActionResult<User> LoginPost()
        {
            throw new NotImplementedException();
        }
    }
}
