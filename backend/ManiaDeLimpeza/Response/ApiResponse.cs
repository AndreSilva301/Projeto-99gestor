namespace ManiaDeLimpeza.Api.Response;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }
    public string TimeStamp { get; set; } = DateTime.UtcNow.ToString("O");

    public ApiResponse(T data, string message = "Request successful")
    {
        Success = true;
        Message = message;
        Data = data;
        Errors = null;
    }

    public ApiResponse(List<string> errors, string message = "Request failed")
    {
        Success = false;
        Message = message; 
        Data = default;
        Errors = errors;
    }

    public ApiResponse(string error, string message = "Request failed")
    {
        Success = false;
        Message = message; 
        Data = default;
        Errors = new List<string> { error };
    }

    public ApiResponse() { }
}
