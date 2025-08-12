namespace UserService.Application.Services;

public interface IAvatarStorageService {

  Task<string> SaveAsync(string fileName, Stream stream, CancellationToken cancellationToken);
  Task DeleteAsync(string relativeUrl, CancellationToken cancellationToken);

}