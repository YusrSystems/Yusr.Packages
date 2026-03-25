using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Yusr.Infrastructure.Persistence.Interceptors
{
    public class SlowQueryInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<SlowQueryInterceptor> _logger;
        private const int SlowThresholdMilliseconds = 500;

        public SlowQueryInterceptor(ILogger<SlowQueryInterceptor> logger) => _logger = logger;

        // For SELECT 
        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            CheckAndLog(command, eventData);
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        // For UPDATE, INSERT, DELETE 
        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            CheckAndLog(command, eventData);
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        // For COUNT, SUM, etc.
        public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
        {
            CheckAndLog(command, eventData);
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        private void CheckAndLog(DbCommand command, CommandExecutedEventData eventData)
        {
            if (eventData.Duration.TotalMilliseconds > SlowThresholdMilliseconds)
            {
                _logger.LogWarning("SLOW SQL ({DurationMs}ms): {CommandText}",
                    eventData.Duration.TotalMilliseconds,
                    command.CommandText);
            }
        }
    }
}