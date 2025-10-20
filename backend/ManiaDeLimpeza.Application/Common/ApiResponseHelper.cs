namespace ManiaDeLimpeza.Api.Response;

public static class ApiResponseHelper
{
    public static ApiResponse<T> SuccessResponse<T>(T data, string message = "Request successful")
    {
        var response = new ApiResponse<T>(data, message);
        response.Success = true;
        response.Message = message;
        return response;
    }

    public static ApiResponse<string> ErrorResponse(List<string> errors, string message = "Request failed")
    {
        var response = new ApiResponse<string>(errors, message);
        response.Success = false;
        response.Message = message;
        response.Errors = errors ?? new List<string>();
        return response;
    }

    public static ApiResponse<string> ErrorResponse(string errorMessage, string message = null)
    {
        var response = new ApiResponse<string>(errorMessage, message ?? errorMessage);
        response.Success = false;
        response.Message = message ?? errorMessage;
        response.Errors = new List<string> { errorMessage };
        return response;
    }
}
