using Abstractions.Commands;

using Ardalis.Result;

using Contracts.Responses.UserService;

using UserService.Application.Services;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.UpdateUserProfile;

public class UpdateUserProfileHandler : ICommandHandler<UpdateUserProfileResponse, UpdateUserProfileCommand> {

  private readonly IUserInfoRepository _userInfoRepository;
  private readonly IAvatarStorageService _avatarStorageService;

  public UpdateUserProfileHandler (IUserInfoRepository userInfoRepository, IAvatarStorageService avatarStorageService) {
    _userInfoRepository = userInfoRepository;
    _avatarStorageService = avatarStorageService;
  }

  public async Task<Result<UpdateUserProfileResponse>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
  {
    // Получение пользователя
    var userResult = await _userInfoRepository.GetUserInfoByIdAsync(command.Request.UserId, cancellationToken);
    if (!userResult.IsSuccess || userResult.Value is null)
      return Result.Error("Пользователь не найден");

    var user = userResult.Value;

    // Обновление основных данных
    user.Username = command.Request.Username ?? user.Username;
    user.AboutMe = command.Request.AboutMe ?? user.AboutMe;
    user.Location = command.Request.Location ?? user.Location;
    user.GitHubUrl = command.Request.GitHubUrl ?? user.GitHubUrl;
    user.WebsiteUrl = command.Request.WebsiteUrl ?? user.WebsiteUrl;
    
    if (command.Request.RemoveAvatar)
    {
      if (!string.IsNullOrEmpty(user.AvatarUrl))
        await _avatarStorageService.DeleteAsync(user.AvatarUrl, cancellationToken);

      user.AvatarUrl = null;
    }
    else if (command.Request.AvatarStream != null && command.Request.AvatarStream.Length > 0)
    {
      if (!string.IsNullOrEmpty(user.AvatarUrl))
        await _avatarStorageService.DeleteAsync(user.AvatarUrl, cancellationToken);

      var ext = Path.GetExtension(command.Request.AvatarStream.FileName)?.ToLowerInvariant();

      var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
      if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
        return Result.Error("Можно загружать только изображения (jpg, jpeg, png, gif, webp)");

      var uniqueFileName = $"{Guid.NewGuid():N}{ext}";

      using var stream = command.Request.AvatarStream.OpenReadStream();
      var newAvatarUrl = await _avatarStorageService.SaveAsync(uniqueFileName, stream, cancellationToken);
      user.AvatarUrl = newAvatarUrl;
    }
    
    var updateResult = await _userInfoRepository.UpdateUserInfoAsync(user, cancellationToken);

    return updateResult.IsSuccess ?
      Result<UpdateUserProfileResponse>.Success(new UpdateUserProfileResponse (true)) :
      Result<UpdateUserProfileResponse>.Error(new ErrorList(updateResult.Errors));
  }

}