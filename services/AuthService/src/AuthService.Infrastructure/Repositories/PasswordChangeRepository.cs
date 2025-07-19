using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class PasswordChangeRepository : IPasswordChangeRepository {

  private readonly AuthServiceDbContext _context;

  public PasswordChangeRepository (AuthServiceDbContext context) {
    _context = context;
  }

  public async Task SaveAsync (Guid userId, string newPassword, string token) {
    _context.PendingPasswordChanges.Add (new PendingPasswordChange {
      UserId = userId, NewPassword = newPassword, Token = token
    });
    await _context.SaveChangesAsync ();
  }

  public async Task<string?> GetPasswordByTokenAsync (string token) {
    return await _context.PendingPasswordChanges
     .Where (r => r.Token == token)
     .Select (r => r.NewPassword)
     .FirstOrDefaultAsync ();
  }

  public async Task RemoveAsync (string token) {
    var request = await _context.PendingPasswordChanges
     .FirstOrDefaultAsync (r => r.Token == token);
    
    if (request != null) {
      _context.PendingPasswordChanges.Remove (request);
      await _context.SaveChangesAsync ();
    }
  }
}