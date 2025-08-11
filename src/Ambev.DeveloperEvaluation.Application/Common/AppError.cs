namespace Ambev.DeveloperEvaluation.Application.Common
{
    public sealed class AppError
    {
        public string Code { get; init; } = default!; 
        public string Message { get; init; } = default!;
        public string? Field { get; init; }
        public static AppError NotFound(string resource, string id)
        => new() { Code = "NotFound", Message = $"{resource} {id} not found." };
    }
}
