using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserService.Domain.Entities;
using UserService.Domain.Filters;

namespace UserService.Domain.Repositories;

public interface IUserInfoRepository {

    Task<Result<(IEnumerable<UserInfo> items, PagedInfo pageInfo)>> GetUsersAsync( PageParams pageParams, SortParams sortParams, CancellationToken token );   

    Task<Result<UserInfo>> GetUserInfoByIdAsync( Guid userId, CancellationToken token );
    Task<Result<UserStatistic>> GetUserStatisticByIdAsync( Guid userId, CancellationToken token );
    Task<Result<UserInfo>> GetFullUserInfoByIdAsync( Guid userId, CancellationToken token );

    Task<Result> CreateUserInfoAsync( Guid userId, string userName, CancellationToken token );

    Task<Result> UpdateUserInfoAsync( UserInfo userInfo, CancellationToken token );
    Task<Result> UpdateUserStatisticAsync( UserStatistic userStatistic, CancellationToken token );   

    Task<Result> DeleteUserInfoAsync( Guid userId, CancellationToken token );

}