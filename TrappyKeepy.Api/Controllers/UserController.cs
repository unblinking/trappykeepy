using Microsoft.AspNetCore.Mvc;
using TrappyKeepy.Domain;
using TrappyKeepy.Domain.Interfaces;


namespace TrappyKeepy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<User>>> ReadAll()
        {
            var users = await userService.ReadAll();
            return Ok(users);
        }
    }
}
