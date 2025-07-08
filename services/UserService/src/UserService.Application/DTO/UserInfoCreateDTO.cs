namespace UserService.Application.DTO;

public class UserInfoCreateDTO {
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;   
}
