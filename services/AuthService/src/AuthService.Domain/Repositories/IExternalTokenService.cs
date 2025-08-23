namespace AuthService.Domain.Repositories;

public interface IExternalTokenService {
    Task<(string Jwt, string Refresh)> IssueAsync( Guid userId, CancellationToken ct = default );
}
