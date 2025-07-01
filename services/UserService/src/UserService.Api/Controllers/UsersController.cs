using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTO;
using UserService.Application.Interfaces;

namespace UserService.Api.Controllers;

[ApiController]
[Route ("users")]
public class UsersController : ControllerBase {

    private readonly IUserInfoService _userInfoService;
    private readonly int _pageSize; // ???? ПОМЕНЯТЬ или ПОЛУЧАТЬ ОТ UI ????

    public UsersController (IUserInfoService userInfoService, IConfiguration configuration) {
        _userInfoService = userInfoService;
        _pageSize = configuration.GetValue<int> ("PaginationSettings:PageSize");
    }

    [HttpGet ("{pageNumber:int}")]
    public async Task<IActionResult> GetUsersByRatingAsync (int pageNumber) {
        if (pageNumber <= 0)
            return BadRequest (new { message = "Page number must be greater than zero." });

        try {
            IEnumerable<UserShortDTO> users = await _userInfoService.GetUsersByRatingAsync (pageNumber, _pageSize);
            return Ok (users);
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }
    }

    [HttpGet ("date/{pageNumber:int}")]
    public async Task<IActionResult> GetUsersByDateAsync (int pageNumber) {
        if (pageNumber <= 0)
            return BadRequest (new { message = "Page number must be greater than zero." });

        try {
            IEnumerable<UserShortDTO> users = await _userInfoService.GetUsersByDateAsync (pageNumber, _pageSize);
            return Ok (users);
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }

        
    }

    [HttpPost("info")]
    public async Task<IActionResult> CreateUserInfoAsync ([FromBody] UserInfoCreateDTO userDto) {
        try {
            bool created = await _userInfoService.CreateUserInfoAsync (userDto);
            return created ? Ok (created) : Conflict (new { message = "User already exists." });
        }
        catch (ArgumentException ex) {
            return BadRequest (new { message = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }
    }

    [HttpPut("info")]
    public async Task<IActionResult> UpdateUserInfoAsync ([FromBody]UserInfoUpdateDTO userDto) {
        try {
            bool updated = await _userInfoService.UpdateUserInfoAsync (userDto);
            return updated ? Ok (updated) : NotFound ();
        }
        catch (ArgumentException ex) {
            return BadRequest (new { message = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }
    }

    [HttpPost("statistic")]
    public async Task<IActionResult> CreateUserStatisticAsync ([FromBody]UserStatisticUpdateDto userDto) {
        try {
            bool created = await _userInfoService.CreateUserStatisticAsync (userDto);
            return created ? Ok (created) : Conflict (new { message = "Statistic already exist" });
        }
        catch (ArgumentException ex) {
            return BadRequest (new { message = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }
    }

    [HttpPut("statistic")]
    public async Task<IActionResult> UpdateUserStatisticAsync ([FromBody]UserStatisticUpdateDto userDto) {
        try {
            bool updated = await _userInfoService.UpdateUserStatisticAsync (userDto);
            return updated ? Ok (updated) : NotFound ();
        }
        catch (ArgumentException ex) {
            return BadRequest (new { message = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUserInfoAsync (Guid userId) {
        if (userId == Guid.Empty)
            return BadRequest (new { message = "User ID cannot be empty." });

        try {
            bool deleted = await _userInfoService.DeleteUserInfoAsync (userId);
            return deleted ? Ok (deleted) : NotFound (new { message = "User not found." });
        }
        catch (ArgumentException ex) {
            return BadRequest (new { message = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode (500, new { message = "Internal server error.", details = ex.Message });
        }
    }

}
