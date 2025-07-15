using CommentService.Domain.Enums;
using Contracts.Commands;

namespace CommentService.Application.Features.GetQuestionComments;

public record GetCommentsCommand(TypeTarget Type, Guid TargetId) : ICommand;