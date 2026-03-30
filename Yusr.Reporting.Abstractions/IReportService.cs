using Yusr.Core.Abstractions.Primitives;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.Reporting.Abstractions
{
    public interface IReportService<TRequest, TRendererData> where TRequest : BaseRequest where TRendererData : BaseRendererData
    {
        public Task<OperationResult<ReportResult<TRendererData>>> GetReportData(TRequest request, JwtClaims jwtClaims, CancellationToken cancellationToken);
    }
}
