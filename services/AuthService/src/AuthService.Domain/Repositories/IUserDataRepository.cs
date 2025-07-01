using Ardalis.Result;

using AuthService.Domain.Entities;

namespace AuthService.Domain.Repositories;

public interface IUserDataRepository {

  Task<Result<UserData>> GetByIdAsync(Guid userId);
  Task<Result<UserData>> GetByEmailAsync(string email);
  Task<Result<Guid>> CreateAsync(UserData user, string? password = null);
  Task<Result<bool>> CheckPasswordAsync(UserData user, string password);
  Task<Result<string>> AddToRoleAsync(UserData user, string role);
  Task<Result<IList<string>>> GetRolesAsync(UserData user);
  Task SaveChangesAsync();

}