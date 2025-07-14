using CommentService.Application.DTO;
using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application.Features.CreateComment;

public record CreateCommentCommand( CreateCommentDTO CreateCommentDTO ) : ICommand;
