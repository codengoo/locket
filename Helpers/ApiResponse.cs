namespace locket.Helpers
{
    public class ApiResponse<T>(string? message, T? data, object? errors)
    {
        public string Message { get; set; } = message ?? "Success";
        public T? Data { get; set; } = data;
        public object? Errors { get; set; } = errors;
    }

    public class ApiResponse(string? message, object? data, object? errors) : ApiResponse<object>(message, data, errors)
    {
    }
}
