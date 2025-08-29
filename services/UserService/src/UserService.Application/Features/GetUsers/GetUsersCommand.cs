using Abstractions.Commands;

using Contracts.Common.Filters;

using PageParams = UserService.Domain.Filters.PageParams;
using SortParams = UserService.Domain.Filters.SortParams;

namespace UserService.Application.Features.GetUsers;

public record GetUsersCommand ( PageParams PageParams, SortParams SortParams, SearchFilter SearchFilter) : ICommand;