using Abstractions.Commands;

using Contracts.Common.Filters;

namespace AnswerService.Application.Features.GetAnswersByUserId;

public record GetAnswersByUserIdCommand (Guid UserId, PageParams PageParams, SortParams SortParams) : ICommand;