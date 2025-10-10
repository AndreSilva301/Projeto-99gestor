namespace ManiaDeLimpeza.Api.Response;

public static class ApiResponseHelper
{
    public static ApiResponse<T> SuccessResponse<T>(T data, string message = "Request successful")
    {
        return new ApiResponse<T>(data, message);
    }
    public static ApiResponse<T> ErrorResponse<T>(List<string> errors, string message = "Request failed")
    {
        return new ApiResponse<T>(errors, message);
    }
    public static ApiResponse<string> ErrorResponse(string errorMessage, string message = "Request failed")
    {
        return new ApiResponse<string>(errorMessage, message);
    }
}
