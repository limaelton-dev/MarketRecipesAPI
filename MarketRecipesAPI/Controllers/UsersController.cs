using MarketRecipesAPI.DTO;
using MarketRecipesAPI.Models;
using MarketRecipesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketRecipesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _userService.GetUsers());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(UserCreateDTO userCreateDTO)
        {
            try
            {
                var user = new User
                {
                    Username = userCreateDTO.Username,
                    Password = userCreateDTO.Password
                };

                var createdUser = await _userService.CreateUser(user);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            try
            {
                var result = await _userService.UpdateUser(id, userUpdateDTO);
                if (!result) return NotFound();

                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
        {
            var token = await _userService.AuthenticateUser(loginDTO.Username, loginDTO.Password);
            if (token == null) return Unauthorized();

            return Ok(new { Token = token });
        }
    }
}
