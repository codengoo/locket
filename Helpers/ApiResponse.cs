namespace locket.Helpers
{
    public class ApiResponse<T>(T? data, string? message = null, object? errors = null)
    {
        public string Message { get; set; } = message ?? "Success";
        public T? Data { get; set; } = data;
        public object? Errors { get; set; } = errors;
    }

    public class ApiResponse(object? data, string? message = null, object? errors = null) : ApiResponse<object>(data, message, errors)
    {
    }
}
