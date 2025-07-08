using Ardalis.Result;

using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository {

  private readonly AuthServiceDbContext _context;

  public RefreshTokenRepository (AuthServiceDbContext context) {
    _context = context;
  }

  public async Task<Result<RefreshToken>> CreateAsync (Guid userId, TimeSpan ttl) {
    var rt = new RefreshToken {
      UserId = userId, 
      Token = Guid.NewGuid ().ToString ("N"), 
      ExpiresAt = DateTime.UtcNow.Add (ttl)
    };
    _context.RefreshTokens.Add (rt);
    await _context.SaveChangesAsync ();
    return rt;
  }

  public async Task<Result<RefreshToken>> GetValidAsync (string token) {
    var refreshToken = await _context.RefreshTokens
     .Where (refreshToken => refreshToken.Token == token)
     .Where (refreshToken => refreshToken.RevokedAt == null && refreshToken.ExpiresAt > DateTime.UtcNow)
     .SingleOrDefaultAsync ();

    if (refreshToken == null) {
      return Result<RefreshToken>.Conflict ("Token not found");
    }

    return Result<RefreshToken>.Success (refreshToken);
  }

  public async Task<Result> RevokeAsync (string token) {
    var refreshToken = await _context.RefreshTokens
     .SingleOrDefaultAsync (refreshToken => refreshToken.Token == token);
    if (refreshToken != null && refreshToken.RevokedAt == null) {
      refreshToken.RevokedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync ();
    }

    return Result.Success ();
  }

  public async Task<Result> RevokeAllAsync (Guid userId) {
    var tokens = await _context.RefreshTokens
     .Where (x => x.UserId == userId && x.RevokedAt == null)
     .ToListAsync ();
    foreach (var rt in tokens) rt.RevokedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync ();
    return Result.Success ();
  }

}