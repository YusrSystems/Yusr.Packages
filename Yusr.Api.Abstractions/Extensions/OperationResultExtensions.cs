using Microsoft.AspNetCore.Mvc;
using Yusr.Core.Abstractions.Enums;
using Yusr.Core.Abstractions.Primitives;

namespace Yusr.Api.Abstractions.Extensions
{
    public static class OperationResultExtensions
    {
        public static ActionResult HandleRequest<T>(this OperationResult<T> operationResult)
        {
            var result = CreatedResult((int)operationResult.ResultType, operationResult.ErrorTitle ?? "حدث خطأ أثناء جلب البيانات.", operationResult.ErrorMessage);

            if (operationResult.ResultType == ResultType.Ok)
                result.Value = operationResult.Result;

            return result;
        }

        private static ObjectResult CreatedResult(int statusCode, string errorTitle, string errorMessage)
        {
            return new ObjectResult(new
            {
                detail = errorMessage,
                statusCode,
                title = errorTitle
            })
            { StatusCode = statusCode };
        }
    }
}
