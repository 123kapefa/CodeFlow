using Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Features.DeleteUser;

public record DeleteUserCommand (Guid UserId) : ICommand;