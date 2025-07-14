using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentService.Application.Features.DeleteCommentById;

public record DeleteCommentByIdCommand(Guid CommentId) : ICommand;
