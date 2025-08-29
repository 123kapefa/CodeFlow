using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Contracts.Common.Filters;

using UserService.Domain.Entities;

using PageParams = UserService.Domain.Filters.PageParams;
using SortDirection = UserService.Domain.Filters.SortDirection;
using SortParams = UserService.Domain.Filters.SortParams;

namespace UserService.Infrastructure.Extensions;

public static class UserInfoExtensions {

    public static async Task<Result<(IEnumerable<UserInfo> items, PagedInfo pageInfo)>> ToPagedAsync( this IQueryable<UserInfo> usersInfo, PageParams pageParams ) {

        int count = usersInfo.Count();

        int page = pageParams.Page ?? 1;
        int pageSize = pageParams.PageSize ?? 36;
        int skip = (page - 1) * pageSize;

        List<UserInfo> items = usersInfo.Skip(skip).Take(pageSize).ToList();
        int totalPages = (int)Math.Ceiling(count / (double)pageSize);

        PagedInfo pagedInfo = new PagedInfo(page, pageSize, totalPages, count);

        return Result<(IEnumerable<UserInfo>, PagedInfo)>.Success((items, pagedInfo));
    }

    public static IQueryable<UserInfo> Sort( this IQueryable<UserInfo> usersInfo, SortParams sortParams ) {

        return sortParams.SortDirection == SortDirection.Descending ?
            usersInfo = usersInfo.OrderByDescending(GetKey(sortParams.OrderBy)) : usersInfo.OrderBy(GetKey(sortParams.OrderBy));
    }

    private static Expression<Func<UserInfo, object>> GetKey( string? orderBy ) {
        if(string.IsNullOrEmpty(orderBy))
            return x => x.UserStatistic.Reputation;

        return orderBy switch {
            nameof(UserInfo.CreatedAt) => x => x.CreatedAt,
            _ => x => x.UserStatistic.Reputation
        };
    }

    public static Expression<Func<UserInfo, bool>> GetSearchFilter (SearchFilter searchFilter) {
        if (String.IsNullOrEmpty (searchFilter.SearchValue))
            return x => true;
        
        string searchValue = searchFilter.SearchValue.ToLower();
        return userInfo => userInfo.Username.ToLower().Contains(searchValue);

    }
}