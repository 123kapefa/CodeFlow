using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTO;
using UserService.Application.Interfaces;

namespace UserService.Api.Controllers;

[ApiController]
[Route ("users")]
public class UsersController : ControllerBase {

    private readonly IUserInfoService _userInfoService;
    private const int PageSize = 20; // ???? ПОМЕНЯТЬ ????

    public UsersController (IUserInfoService userInfoService) {
        _userInfoService = userInfoService;
    }

    [HttpGet ("{pageNumber:int}")]
    public async Task<IActionResult> GetUsersByRatingAsync (int pageNumber) {
        if (pageNumber <= 0)
            return BadRequest (new { message = "Page number must be greater than zero." });

        IEnumerable<UserShortDTO> users = await _userInfoService.GetUsersByRatingAsync (pageNumber, PageSize);

        return Ok (users);
    }

    [HttpGet ("date/{pageNumber:int}")]
    public async Task<IActionResult> GetUsersByDateAsync (int pageNumber) {
        if (pageNumber <= 0)
            return BadRequest (new { message = "Page number must be greater than zero." });

        IEnumerable<UserShortDTO> users = await _userInfoService.GetUsersByDateAsync (pageNumber, PageSize);
        return Ok (users);
    }

    [HttpPost("info")]
    public async Task<IActionResult> CreateUserInfoAsync ([FromBody] UserInfoCreateDTO userDto) {
        bool created = await _userInfoService.CreateUserInfoAsync (userDto);
        return created ? Ok (created) : Conflict (new { message = "User already exists" });
    }

    [HttpPut("info")]
    public async Task<IActionResult> UpdateUserInfoAsync ([FromBody]UserInfoUpdateDTO userDto) {
        bool updated = await _userInfoService.UpdateUserInfoAsync (userDto);
        return updated ? Ok (updated) : NotFound ();
    }

    [HttpPost("statistic")]
    public async Task<IActionResult> CreateUserStatisticAsync ([FromBody]UserStatisticUpdateDto userDto) {
        bool created = await _userInfoService.CreateUserStatisticAsync (userDto);
        return created ? Ok (created) : Conflict (new { message = "Statistic already exist" });
    }

    [HttpPut("statistic")]
    public async Task<IActionResult> UpdateUserStatisticAsync ([FromBody]UserStatisticUpdateDto userDto) {
        bool updated = await _userInfoService.UpdateUserStatisticAsync (userDto);
        return updated ? Ok (updated) : NotFound ();
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUserInfoAsync (Guid userId) {
        bool deleted = await _userInfoService.DeleteUserInfoAsync (userId);
        return deleted ? Ok(deleted) : NotFound ();
    }

}
