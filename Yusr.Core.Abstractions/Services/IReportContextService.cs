using Yusr.Core.Abstractions.Models;

namespace Yusr.Core.Abstractions.Services
{
    public interface IReportContextService
    {
        Task<ReportContext?> GetContextAsync(long userId, CancellationToken cancellationToken);
    }
}
