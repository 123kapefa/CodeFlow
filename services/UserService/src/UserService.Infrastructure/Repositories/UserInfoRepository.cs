using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Filters;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Extensions;

namespace UserService.Infrastructure.Repositories;

public class UserInfoRepository : IUserInfoRepository {
    private readonly UserServiceDbContext _dbContext;

    public UserInfoRepository( UserServiceDbContext dbContext ) {
        _dbContext = dbContext;
    }


    /// <summary> Получение информации о пользователе по ID </summary>
    public async Task<Result<UserInfo>> GetUserInfoByIdAsync( Guid userId, CancellationToken token ) {
        if(userId == Guid.Empty) {
            return Result<UserInfo>.Error("ID пользователя не может быть пустым");
        }

        UserInfo? userInfo = await _dbContext.UsersInfos.FirstOrDefaultAsync(x => x.UserId == userId, token);
        if(userInfo == null)
            return Result<UserInfo>.NotFound("Пользователь не найден");

        return Result<UserInfo>.Success(userInfo);
    }


    /// <summary> Получение статистики пользователя по ID </summary>
    public async Task<Result<UserStatistic>> GetUserStatisticByIdAsync( Guid userId, CancellationToken token ) {
        if(userId == Guid.Empty) {
            return Result<UserStatistic>.Error("ID пользователя не может быть пустым");
        }

        UserStatistic? userStatistic = await _dbContext.UsersStatistic.FirstOrDefaultAsync(x => x.UserId == userId, token);
        if(userStatistic == null)
            return Result<UserStatistic>.NotFound("Пользователь не найден");

        return Result<UserStatistic>.Success(userStatistic);
    }


    /// <summary> Получение полной информации о пользователе по ID </summary>
    public async Task<Result<UserInfo>> GetFullUserInfoByIdAsync( Guid userId, CancellationToken token ) {
        if(userId == Guid.Empty) {
            return Result<UserInfo>.Error("ID пользователя не может быть пустым");
        }

        UserInfo? userInfo = await _dbContext.UsersInfos
            .Include(u => u.UserStatistic)
            .FirstOrDefaultAsync(x => x.UserId == userId, token);

        if(userInfo == null)
            return Result<UserInfo>.NotFound("Пользователь не найден");

        return Result<UserInfo>.Success(userInfo);
    }


    /// <summary> Получение пользователей по рейтингу/дате регистрации </summary>
    public async Task<Result<(IEnumerable<UserInfo> items, PagedInfo pageInfo)>> GetUsersAsync( PageParams pageParams, SortParams sortParams, CancellationToken token ) {   

        try {

            var users = await _dbContext.UsersInfos
                .Include(u => u.UserStatistic)
                .Sort(sortParams)
                .ToPagedAsync(pageParams);
                   

            return Result<(IEnumerable<UserInfo> items, PagedInfo pageInfo)>.Success(users);
        }
        catch(Exception) {
            return Result<(IEnumerable<UserInfo> items, PagedInfo pageInfo)>.Error("Ошибка базы данных");
        }
    }  


    /// <summary> Создание UserInfo </summary>    
    public async Task<Result> CreateUserInfoAsync( Guid userId, string userName, CancellationToken token ) {        

        if(userId == Guid.Empty) {
            return Result.Error("ID пользователя не может быть пустым");
        }

        bool exists = await _dbContext.UsersInfos.AnyAsync(u => u.UserId == userId);
        if(exists)
            return Result.Error("Запись о пользователе с таким ID уже существует");

        UserInfo userInfo = UserInfo.Create(userId, userName);

        _dbContext.UsersInfos.Add(userInfo);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    
    /// <summary> Обновление UserInfo </summary>   
    public async Task<Result> UpdateUserInfoAsync( UserInfo user, CancellationToken token ) { 
        if(user.UserId == Guid.Empty) {
            return Result.Error("ID пользователя не может быть пустым");
        }

        _dbContext.UsersInfos.Update(user);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    
    /// <summary> Обновление UserStatistic </summary>   
    public async Task<Result> UpdateUserStatisticAsync( UserStatistic userStatistic, CancellationToken token ) {
        if(userStatistic.UserId == Guid.Empty) {
            return Result.Error("ID пользователя не может быть пустым");
        }

        _dbContext.UsersStatistic.Update(userStatistic);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    
    /// <summary> Удаление из UserInfo и UserStatistic </summary>
    public async Task<Result> DeleteUserInfoAsync( Guid userId, CancellationToken token ) {
        if(userId == Guid.Empty) {
            return Result.Error("ID пользователя не может быть пустым");
        }

        UserStatistic? user = await _dbContext.UsersStatistic
            .Where(u => u.UserId == userId)            
            .FirstOrDefaultAsync();

        if(user == null) {
            return Result.Error("пользователь с таким ID не найден");
        }

        _dbContext.UsersStatistic.Remove(user);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
