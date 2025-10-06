namespace ManiaDeLimpeza.Api.Response;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set;}
    public List<string> Errors { get; set; } = new List<string>();
    public string TimeStamp { get; set; } = DateTime.UtcNow.ToString("O");

    public ApiResponse(T data, string message)
    {
        Success = true;
        Message = message;
        Data = data;
        Errors = new List<string>();
        TimeStamp = DateTime.UtcNow.ToString("O");
    }

    public ApiResponse(List<string> errors, string message)
    {
        Success = false;
        Message = message;
        Data = default;
        Errors = errors;
        TimeStamp = DateTime.UtcNow.ToString("O");
    }
}
