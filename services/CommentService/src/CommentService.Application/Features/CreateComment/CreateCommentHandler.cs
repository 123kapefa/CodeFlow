using Ardalis.Result;
using CommentService.Domain.Entities;
using CommentService.Domain.Enums;
using CommentService.Domain.Repositories;
using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application.Features.CreateComment;

public class CreateCommentHandler : ICommandHandler<CreateCommentCommand> {

    private readonly ICommentRepository _commentRepository;

    public CreateCommentHandler( ICommentRepository commentRepository ) {
        _commentRepository = commentRepository;
    }

    public async Task<Result> Handle( CreateCommentCommand command, CancellationToken token ) {

        Comment comment = new Comment { 
            AuthorId = command.CreateCommentDTO.AuthorId,
            Content = command.CreateCommentDTO.Content,
            CreatedAt = DateTime.UtcNow,
            Type =  command.CreateCommentDTO.Type,
            TargetId = command.CreateCommentDTO.TargetId,
        };

        Result result = await _commentRepository.CreateCommentAsync(comment, token);

        return result.IsSuccess ? Result.Success() : Result.Error(new ErrorList(result.Errors));
    }
}
