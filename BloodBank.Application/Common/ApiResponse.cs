namespace BloodBank.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(bool success, string? message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        //public static ApiResponse<T> Ok(T Data)
        //{
        //    return new ApiResponse<T>
        //    {
        //        Success = true,
        //        Message = "Success",
        //        Data = Data
        //    };
        //}

        //public static ApiResponse<T> Fail(string message)
        //{
        //    return new ApiResponse<T>
        //    {
        //        Success = false,
        //        Message = message,
        //    };
        //}
    }
}
