using Ardalis.Result;
using FluentValidation.Results;

namespace AuthService.Application.Extensions
{
public static class ValidationResultExtensions
{
  public static Result<T> ToInvalidResult<T>(
    this ValidationResult validationResult,
    string errorCodePrefix) {
    
    var errors = validationResult.Errors
     .Select(e => new ValidationError(
        $"{errorCodePrefix}",
        e.ErrorCode,
        e.ErrorMessage,
        ValidationSeverity.Error));

    return Result<T>.Invalid(errors);
  }
}
}