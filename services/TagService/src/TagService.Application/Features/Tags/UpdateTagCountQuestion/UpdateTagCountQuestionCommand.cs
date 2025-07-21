using Abstractions.Commands;

namespace TagService.Application.Features.Tags.UpdateTagCountQuestion;

//TODO УДАЛИТЬ?? ИСПОЛЬЗОВАТЬ UpdateTagRequestCommand???
public record UpdateTagCountQuestionCommand(string Name, int Count): ICommand;
