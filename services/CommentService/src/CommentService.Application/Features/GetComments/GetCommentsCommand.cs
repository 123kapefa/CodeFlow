using Abstractions.Commands;

using CommentService.Domain.Enums;

namespace CommentService.Application.Features.GetComments;

public record GetCommentsCommand(TypeTarget Type, Guid TargetId) : ICommand;