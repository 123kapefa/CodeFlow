using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
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

    
    public async Task<IEnumerable<UserShortDTO>> GetUsersByRatingAsync (int pageNumber, int pageSize) {
      
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

        bool exists = await _dbContext.UsersInfos.AnyAsync (u => u.Id == userDto.UserId);
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
       
        var exist = await _dbContext.UsersInfos.AnyAsync (u => u.Id == userDto.UserId);
        if(exist) return false;

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

        UserStatistic userStat = await _dbContext.UsersStatistic.FirstOrDefaultAsync (u => u.UserId == userDto.UserId);
        if(userStat == null) return false;

        userStat.LastVisitAt = DateTime.UtcNow;
        userStat.VisitCount += userDto.VisitCount;
        userStat.Reputation += userDto.Reputation;

        await _dbContext.SaveChangesAsync ();

        return true;
    }

    // Удаление из UserInfo и UserStatistic
    public async Task<bool> DeleteUserInfoAsync (Guid userId) {

        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync ();

        try {
            UserInfo userInfo = await _dbContext.UsersInfos.FirstOrDefaultAsync (u => u.UserId == userId);
            if (userInfo == null) return false;

            UserStatistic userStat = await _dbContext.UsersStatistic.FirstOrDefaultAsync (u => u.UserId == userId);
            if (userInfo == null) return false;

            _dbContext.UsersInfos.Remove (userInfo);
            _dbContext.UsersStatistic.Remove (userStat);

            await _dbContext.SaveChangesAsync ();
            await transaction.CommitAsync ();

            return true;
        }
        catch {
            await transaction.RollbackAsync ();
            return false;
        }
    }

}
