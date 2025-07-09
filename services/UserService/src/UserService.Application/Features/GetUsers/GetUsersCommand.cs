using Contracts.Commands;
using UserService.Domain.Filters;

namespace UserService.Application.Features.GetUsers;

public record GetUsersCommand ( PageParams PageParams, SortParams SortParams) : ICommand;