using Contracts.DTOs.QuestionService;

namespace Contracts.Requests.ApiGateway;

public record CreateQuestionRequest (CreateQuestionDTO QuestionDto);