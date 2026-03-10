using Microsoft.EntityFrameworkCore;
using Npgsql;
using Yusr.Core.Abstractions.Constants;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Core.Abstractions.Services;

namespace Yusr.Persistence.Services
{
    public class ExceptionService : IExceptionService
    {
        public OperationResult<T> Map<T>(Exception ex, string entityName, string? errorTitle = null)
        {
            var baseEx = ex.GetBaseException();

            return baseEx switch
            {
                DbUpdateConcurrencyException => OperationResult<T>.Conflict(ErrorMessages.UpdateConcurrencyError(entityName)),

                PostgresException pgEx when pgEx.SqlState == "23505" => OperationResult<T>.Conflict(ErrorMessages.FailedToAdd(entityName) + " - القيمة موجودة مسبقاً"),

                PostgresException pgEx when pgEx.SqlState == "23503" => OperationResult<T>.Conflict(ErrorMessages.FailedToDelete(entityName) + " - يوجد بيانات مرتبطة"),

                PostgresException pgEx when pgEx.SqlState == "23001" => OperationResult<T>.Conflict(ErrorMessages.FailedToDelete(entityName) + " - يوجد بيانات مرتبطة"),

                _ => OperationResult<T>.InternalError(errorTitle ?? ErrorMessages.OperationFailed, baseEx.Message)
            };
        }
    }
}
