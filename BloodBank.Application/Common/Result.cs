namespace BloodBank.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public string Error { get; private set; }
        public T Value { get; private set; }

        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, T value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null);
        public static Result<T> Failure(string error) => new Result<T>(false, default, error);
    }
}
