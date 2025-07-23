namespace AuthService.Domain.Repositories;

public interface IPasswordChangeRepository {

  Task SaveAsync(Guid userId, string newPassword, string token);
  Task<string?> GetPasswordByTokenAsync(string token);
  Task RemoveAsync(string token);

}