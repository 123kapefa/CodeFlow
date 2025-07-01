using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using UserService.Application.DTO;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Services;

public class UserInfoService : IUserInfoService {

    private readonly UserServiceDbContext _dbContext;

    public UserInfoService (UserServiceDbContext dbContext) {
        _dbContext = dbContext;
    }

    // Валидация для имени пользователя
    private bool IsValidUsername (string username) {
        return !string.IsNullOrEmpty (username) && username.Length >= 3 && username.Length <= 50;
    }

    // Валидация для VisitCount
    private bool IsValidVisitCount (int visitCount) {
        return visitCount >= 0;  
    }

    // Валидация для URL
    private bool IsValidUrl (string url) {
        return Uri.IsWellFormedUriString (url, UriKind.Absolute);
    }


    public async Task<IEnumerable<UserShortDTO>> GetUsersByRatingAsync (int pageNumber, int pageSize) {

        if (pageNumber <= 0) throw new ArgumentException ("Page number must be greater than zero.");

        List<UserShortDTO> users = await _dbContext.UsersInfos
            .Join(
                _dbContext.UsersStatistic,
                info => info.UserId,
                stat => stat.UserId,
                (info, stat) => new { Info = info, Stat = stat}
            )
            .OrderByDescending(u => u.Stat.Reputation)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserShortDTO {
                Username = u.Info.Username,
                Location = u.Info.Location,
                AboutMe = u.Info.AboutMe,
                AvatarUrl = u.Info.AvatarUrl,
                Reputation = u.Stat.Reputation,
                Tags = new List<string>() // ПОКА ТАК - ПОТОМ ПОЛУЧАТЬ ТЭГИ ИЗ ДРУГОГО СЕРВИСА
            }).ToListAsync();

        return users;
    }

    // Получение пользователей по дате регистрации(возрастание) 
    public async Task<IEnumerable<UserShortDTO>> GetUsersByDateAsync (int pageNumber, int pageSize) {

        if (pageNumber <= 0) throw new ArgumentException ("Page number must be greater than zero.");

        List<UserShortDTO> users = await _dbContext.UsersInfos
            .Join (
                _dbContext.UsersStatistic,
                info => info.UserId,
                stat => stat.UserId,
                (info, stat) => new { Info = info, Stat = stat }
            )
            .OrderBy (u => u.Info.CreatedAt)
            .Skip ((pageNumber - 1) * pageSize)
            .Take (pageSize)
            .Select (u => new UserShortDTO {
                Username=u.Info.Username,
                Location = u.Info.Location,
                AboutMe = u.Info.AboutMe,
                AvatarUrl = u.Info.AvatarUrl,
                Reputation=u.Stat.Reputation,
                Tags = new List<string>() // ПОКА ТАК - ПОТОМ ПОЛУЧАТЬ ТЭГИ ИЗ ДРУГОГО СЕРВИСА
            }).ToListAsync ();

        return users;
    } 

    // Создание UserInfo
    public async Task<bool> CreateUserInfoAsync (UserInfoCreateDTO userDto) {

        if (!IsValidUsername (userDto.Username)) {
            throw new ArgumentException ("Username cannot be empty and must be between 3 and 50 characters.");
        }

        if (userDto.UserId == Guid.Empty) {
            throw new ArgumentException ("User ID cannot be empty.");
        }

        bool exists = await _dbContext.UsersInfos.AnyAsync (u => u.UserId == userDto.UserId);
        if (exists) return false;

        UserInfo userInfo = new UserInfo {
            UserId = userDto.UserId,
            Username = userDto.Username, // БЕРЕМ ПРИ РЕГИСТРАЦИИ ???            
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.UsersInfos.Add (userInfo);
        await _dbContext.SaveChangesAsync ();

        return true;
    }

    // Обновление UserInfo
    public async Task<bool> UpdateUserInfoAsync (UserInfoUpdateDTO userDto) {

        if (!IsValidUsername (userDto.Username)) {
            throw new ArgumentException ("Username cannot be empty and must be between 3 and 50 characters.");
        }

        if (!string.IsNullOrEmpty (userDto.AvatarUrl) && !IsValidUrl (userDto.AvatarUrl)) {
            throw new ArgumentException ("Invalid Avatar URL.");
        }

        if (!string.IsNullOrEmpty (userDto.WebsiteUrl) && !IsValidUrl (userDto.WebsiteUrl)) {
            throw new ArgumentException ("Invalid Website URL.");
        }

        if (!string.IsNullOrEmpty (userDto.GitHubUrl) && !IsValidUrl (userDto.GitHubUrl)) {
            throw new ArgumentException ("Invalid GitHub URL.");
        }

        if (userDto.UserId == Guid.Empty) {
            throw new ArgumentException ("User ID cannot be empty.");
        }

        UserInfo userInfo = await _dbContext.UsersInfos.FirstOrDefaultAsync (u => u.UserId == userDto.UserId);
        if (userInfo == null) return false;
               
        userInfo.Username = userDto.Username;
        userInfo.AvatarUrl = userDto.AvatarUrl;
        userInfo.Location = userDto.Location;
        userInfo.WebsiteUrl = userDto.WebsiteUrl;
        userInfo.GitHubUrl = userDto.GitHubUrl;
        userInfo.AboutMe = userDto.AboutMe;

        await _dbContext.SaveChangesAsync ();

        return true;
    }

    // Создание UserStatistic
    public async Task<bool> CreateUserStatisticAsync (UserStatisticUpdateDto userDto) {

        if (!IsValidVisitCount (userDto.VisitCount)) {
            throw new ArgumentException ("VisitCount must be a non-negative number.");
        }

        if (userDto.UserId == Guid.Empty) {
            throw new ArgumentException ("User ID cannot be empty.");
        }

        bool userExists = await _dbContext.UsersInfos.AnyAsync (u => u.UserId == userDto.UserId); 
        if (!userExists) {
            throw new ArgumentException ("User does not exist.");
        }

        bool statisticExists = await _dbContext.UsersStatistic.AnyAsync (s => s.UserId == userDto.UserId);
        if (statisticExists) {
            throw new ArgumentException ("User statistic already exists.");
        }

        UserStatistic userStat = new UserStatistic { 
            UserId = userDto.UserId,
            LastVisitAt = DateTime.UtcNow,
            VisitCount = userDto.VisitCount, // ПЕРЕДАВАТЬ СРАЗУ 1 ??
            Reputation = 0
        };

        _dbContext.UsersStatistic.Add (userStat);
        await _dbContext.SaveChangesAsync ();

        return true;
    }

    // Обновление UserStatistic
    public async Task<bool> UpdateUserStatisticAsync (UserStatisticUpdateDto userDto) {

        if (!IsValidVisitCount (userDto.VisitCount)) {
            throw new ArgumentException ("VisitCount must be a non-negative number.");
        }

        if (userDto.UserId == Guid.Empty) {
            throw new ArgumentException ("User ID cannot be empty.");
        }

        UserStatistic userStat = await _dbContext.UsersStatistic.FirstOrDefaultAsync (u => u.UserId == userDto.UserId);
        if(userStat == null) return false;
     
        if(userDto.LastVisitAt != null)
            userStat.LastVisitAt = userDto.LastVisitAt;
        userStat.VisitCount += userDto.VisitCount;
        userStat.Reputation += userDto.Reputation;

        await _dbContext.SaveChangesAsync ();

        return true;
    }

    // Удаление из UserInfo и UserStatistic
    public async Task<bool> DeleteUserInfoAsync (Guid userId) {

        if (userId == Guid.Empty) {
            throw new ArgumentException ("User ID cannot be empty.");
        }

        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync ();

        try {
            UserInfo userInfo = await _dbContext.UsersInfos.FirstOrDefaultAsync (u => u.UserId == userId);
            if (userInfo == null) {
                throw new ArgumentException ($"User with ID {userId} not found.");
            }

            UserStatistic userStat = await _dbContext.UsersStatistic.FirstOrDefaultAsync (u => u.UserId == userId);
            if (userStat == null) {
                throw new ArgumentException ($"User statistic for ID {userId} not found.");
            }

            _dbContext.UsersInfos.Remove (userInfo);
            _dbContext.UsersStatistic.Remove (userStat);

            await _dbContext.SaveChangesAsync ();
            await transaction.CommitAsync ();

            return true;
        }
        catch (Exception ex) {
            await transaction.RollbackAsync ();
            throw new InvalidOperationException ("Error deleting user information.", ex); 
        }
    }

}