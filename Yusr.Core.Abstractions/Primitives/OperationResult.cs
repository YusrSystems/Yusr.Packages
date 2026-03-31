using Yusr.Core.Abstractions.Constants;
using Yusr.Core.Abstractions.Enums;

namespace Yusr.Core.Abstractions.Primitives
{
    public class OperationResult<T>
    {
        private readonly T? _result;

        public ResultType ResultType { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public string ErrorTitle { get; private set; } = string.Empty;
        public string WarningMessage { get; private set; } = string.Empty;

        public bool Succeeded => ResultType == ResultType.Ok;

        public T Result
        {
            get
            {
                if (!Succeeded)
                {
                    throw new InvalidOperationException($"محاولة غير آمنة للوصول للنتيجة. الحالة: {ResultType}. الخطأ: {ErrorMessage}");
                }

                return _result!;
            }
        }

        private OperationResult(T result)
        {
            _result = result;
            ResultType = ResultType.Ok;
        }

        private OperationResult(ResultType type, string title, string message)
        {
            ResultType = type;
            ErrorTitle = title;
            ErrorMessage = message;
            _result = default;
        }

        public static OperationResult<T> Ok(T result) => new(result);

        public static OperationResult<T> BadRequest(string message) => new(ResultType.BadRequest, ErrorMessages.BadRequest(), message);

        public static OperationResult<T> NotFound(string message = "طلبك غير موجود") => new(ResultType.NotFound, ErrorMessages.NotFound, message);

        public static OperationResult<T> InternalError(string title, string message) => new(ResultType.InternalError, title, message);

        public static OperationResult<T> ValidationError(string title, string message) => new(ResultType.UnprocessableEntity, title, message);

        public static OperationResult<T> ValidationWarning(string title, string message) => new(ResultType.PreconditionFailed, title, message);

        public static OperationResult<T> Cancelled() => new(ResultType.Cancelled, string.Empty, string.Empty);

        public static OperationResult<T> Conflict(string errorMessage) => new(ResultType.Conflict, ErrorMessages.Conflict, errorMessage);

        public static OperationResult<T> CopyErrorsFrom<TFrom>(OperationResult<TFrom> other)
        {
            return new OperationResult<T>(other.ResultType, other.ErrorTitle, other.ErrorMessage);
        }
    }
}
