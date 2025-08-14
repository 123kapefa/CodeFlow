using Ardalis.Result;

using Contracts.Common.Filters;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Extensions;

using PageParams = UserService.Domain.Filters.PageParams;
using SortParams = UserService.Domain.Filters.SortParams;

namespace UserService.Infrastructure.Repositories;

public class UserInfoRepository : IUserInfoRepository {

  private readonly UserServiceDbContext _dbContext;
  private readonly ILogger<UserInfoRepository> _logger;

  public UserInfoRepository (UserServiceDbContext dbContext, ILogger<UserInfoRepository> logger) {
    _dbContext = dbContext;
    _logger = logger;
  }

  /// <summary> Получение информации о пользователе по ID </summary>
  public async Task<Result<UserInfo>> GetUserInfoByIdAsync (Guid userId, CancellationToken token) {
    _logger.LogInformation ("GetUserInfoByIdAsync started. UserId: {UserId}", userId);

    if (userId == Guid.Empty) {
      _logger.LogWarning ("GetUserInfoByIdAsync: пустой UserId");
      return Result<UserInfo>.Error ("ID пользователя не может быть пустым");
    }

    UserInfo? userInfo = await _dbContext.UsersInfos.FirstOrDefaultAsync (x => x.UserId == userId, token);

    if (userInfo == null) {
      _logger.LogWarning ("GetUserInfoByIdAsync: пользователь {UserId} не найден", userId);
      return Result<UserInfo>.NotFound ("Пользователь не найден");
    }

    _logger.LogInformation ("GetUserInfoByIdAsync: пользователь {UserId} успешно найден", userId);
    return Result<UserInfo>.Success (userInfo);
  }

  public async Task<Result<IEnumerable<UserInfo>>> GetUsersByIdsAsync (IEnumerable<Guid> userIds, CancellationToken token) {
    _logger.LogInformation ("GetUsersByIdsAsync started. UserId");

    var users = await _dbContext.UsersInfos
     .Where (x => userIds.Contains (x.UserId))
     .Include (u => u.UserStatistic)
     .ToListAsync (token);

    if (users.Count == 0) {
      _logger.LogWarning ("GetUsersByIdsAsync: пользователи не найдены");
      return Result<IEnumerable<UserInfo>>.NotFound ("Пользователь не найден");
    }

    _logger.LogInformation ("GetUsersByIdsAsync: пользователи успешно найдены");
    return Result<IEnumerable<UserInfo>>.Success (users);
  }

  /// <summary> Получение статистики пользователя по ID </summary>
  public async Task<Result<UserStatistic>> GetUserStatisticByIdAsync (Guid userId, CancellationToken token) {
    _logger.LogInformation ("GetUserStatisticByIdAsync started. UserId: {UserId}", userId);

    if (userId == Guid.Empty) {
      _logger.LogWarning ("GetUserStatisticByIdAsync: пустой UserId");
      return Result<UserStatistic>.Error ("ID пользователя не может быть пустым");
    }

    UserStatistic? userStatistic = await _dbContext.UsersStatistic.FirstOrDefaultAsync (x => x.UserId == userId, token);

    if (userStatistic == null) {
      _logger.LogWarning ("GetUserStatisticByIdAsync: статистика пользователя {UserId} не найдена", userId);
      return Result<UserStatistic>.NotFound ("Пользователь не найден");
    }

    _logger.LogInformation ("GetUserStatisticByIdAsync: статистика пользователя {UserId} успешно получена", userId);
    return Result<UserStatistic>.Success (userStatistic);
  }

  /// <summary> Получение полной информации о пользователе по ID </summary>
  public async Task<Result<UserInfo>> GetFullUserInfoByIdAsync (Guid userId, CancellationToken token) {
    _logger.LogInformation ("GetFullUserInfoByIdAsync started. UserId: {UserId}", userId);

    if (userId == Guid.Empty) {
      _logger.LogWarning ("GetFullUserInfoByIdAsync: пустой UserId");
      return Result<UserInfo>.Error ("ID пользователя не может быть пустым");
    }

    UserInfo? userInfo = await _dbContext.UsersInfos.Include (u => u.UserStatistic)
     .FirstOrDefaultAsync (x => x.UserId == userId, token);

    if (userInfo == null) {
      _logger.LogWarning ("GetFullUserInfoByIdAsync: пользователь {UserId} не найден", userId);
      return Result<UserInfo>.NotFound ("Пользователь не найден");
    }

    _logger.LogInformation ("GetFullUserInfoByIdAsync: полная информация по пользователю {UserId} успешно получена"
      , userId);
    return Result<UserInfo>.Success (userInfo);
  }

  /// <summary> Получение пользователей по рейтингу/дате регистрации </summary>
  public async Task<Result<(IEnumerable<UserInfo> items, PagedInfo pageInfo)>> GetUsersAsync (
    PageParams pageParams
    , SortParams sortParams
    , SearchFilter searchFilter
    , CancellationToken token) {
    _logger.LogInformation ("GetUsersAsync started. PageParams: {@PageParams}, SortParams: {@SortParams}", pageParams
      , sortParams);

    try {
      var users = await _dbContext.UsersInfos
       .Include (u => u.UserStatistic)
       .Where(UserInfoExtensions.GetSearchFilter (searchFilter))
       .Sort (sortParams)
       .ToPagedAsync (pageParams);

      _logger.LogInformation ("GetUsersAsync: получено {Count} пользователей", users.Value.items.Count ());
      return Result<(IEnumerable<UserInfo> items, PagedInfo pageInfo)>.Success (users);
    }
    catch (Exception ex) {
      _logger.LogError (ex, "GetUsersAsync: ошибка базы данных");
      return Result<(IEnumerable<UserInfo> items, PagedInfo pageInfo)>.Error ("Ошибка базы данных");
    }
  }

  /// <summary> Создание UserInfo </summary>
  public async Task<Result> CreateUserInfoAsync (Guid userId, string userName, CancellationToken token) {
    _logger.LogInformation ("CreateUserInfoAsync started. UserId: {UserId}, UserName: {UserName}", userId, userName);

    if (userId == Guid.Empty) {
      _logger.LogWarning ("CreateUserInfoAsync: пустой UserId");
      return Result.Error ("ID пользователя не может быть пустым");
    }

    bool exists = await _dbContext.UsersInfos.AnyAsync (u => u.UserId == userId);

    if (exists) {
      _logger.LogWarning ("CreateUserInfoAsync: запись для пользователя {UserId} уже существует", userId);
      return Result.Error ("Запись о пользователе с таким ID уже существует");
    }

    UserInfo userInfo = UserInfo.Create (userId, userName);

    _dbContext.UsersInfos.Add (userInfo);
    await _dbContext.SaveChangesAsync ();

    _logger.LogInformation ("CreateUserInfoAsync: пользователь {UserId} успешно создан", userId);
    return Result.Success ();
  }

  /// <summary> Обновление UserInfo </summary>
  public async Task<Result> UpdateUserInfoAsync (UserInfo user, CancellationToken token) {
    _logger.LogInformation ("UpdateUserInfoAsync started. UserId: {UserId}", user.UserId);

    if (user.UserId == Guid.Empty) {
      _logger.LogWarning ("UpdateUserInfoAsync: пустой UserId");
      return Result.Error ("ID пользователя не может быть пустым");
    }

    _dbContext.UsersInfos.Update (user);
    await _dbContext.SaveChangesAsync ();

    _logger.LogInformation ("UpdateUserInfoAsync: пользователь {UserId} успешно обновлён", user.UserId);
    return Result.Success ();
  }

  /// <summary> Обновление UserStatistic </summary>
  public async Task<Result> UpdateUserStatisticAsync (UserStatistic userStatistic, CancellationToken token) {
    _logger.LogInformation ("UpdateUserStatisticAsync started. UserId: {UserId}", userStatistic.UserId);

    if (userStatistic.UserId == Guid.Empty) {
      _logger.LogWarning ("UpdateUserStatisticAsync: пустой UserId");
      return Result.Error ("ID пользователя не может быть пустым");
    }

    _dbContext.UsersStatistic.Update (userStatistic);
    await _dbContext.SaveChangesAsync ();

    _logger.LogInformation ("UpdateUserStatisticAsync: статистика пользователя {UserId} успешно обновлена"
      , userStatistic.UserId);
    return Result.Success ();
  }

  /// <summary> Удаление из UserInfo и UserStatistic </summary>
  public async Task<Result> DeleteUserInfoAsync (Guid userId, CancellationToken token) {
    _logger.LogInformation ("DeleteUserInfoAsync started. UserId: {UserId}", userId);

    if (userId == Guid.Empty) {
      _logger.LogWarning ("DeleteUserInfoAsync: пустой UserId");
      return Result.Error ("ID пользователя не может быть пустым");
    }

    UserStatistic? user = await _dbContext.UsersStatistic.Where (u => u.UserId == userId).FirstOrDefaultAsync ();

    if (user == null) {
      _logger.LogWarning ("DeleteUserInfoAsync: пользователь {UserId} не найден", userId);
      return Result.Error ("пользователь с таким ID не найден");
    }

    _dbContext.UsersStatistic.Remove (user);
    await _dbContext.SaveChangesAsync ();

    _logger.LogInformation ("DeleteUserInfoAsync: пользователь {UserId} успешно удалён", userId);
    return Result.Success ();
  }

}