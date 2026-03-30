using Microsoft.Extensions.Caching.Memory;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Core.Abstractions.Services;
using Yusr.Identity.Abstractions.Primitives;
using Yusr.Storage.Abstractions.Services;

namespace Yusr.Reporting.Abstractions
{
    public abstract class BaseReportService<TRequest, TRendererData>(
        IReportContextService reportContextService,
        IFilesStorage filesStorage, 
        IReportRenderer<TRendererData> renderer, 
        IMemoryCache cache
    ) : IReportService<TRequest, TRendererData> where TRendererData : BaseRendererData where TRequest : BaseRequest
    {
        protected readonly IReportContextService _reportContextService = reportContextService;
        protected readonly IFilesStorage _filesStorage = filesStorage;
        protected readonly IReportRenderer<TRendererData> _renderer = renderer;
        protected readonly IMemoryCache _cache = cache;

        protected record ReportGenerationContext(
            JwtClaims Claims,
            CancellationToken CancellationToken
        );

        public async Task<OperationResult<ReportResult<TRendererData>>> GetReportData(TRequest request, JwtClaims jwtClaims, CancellationToken cancellationToken)
        {
            try
            {
                var reportContext = new ReportGenerationContext(jwtClaims, cancellationToken);

                var rendererData = await ExecuteReportLogic(jwtClaims.TenantId, jwtClaims.UserId, reportContext, request);

                if (!rendererData.Succeeded)
                    return OperationResult<ReportResult<TRendererData>>.CopyErrorsFrom(rendererData);

                return OperationResult<ReportResult<TRendererData>>.Ok(new ReportResult<TRendererData>(_renderer.Render(rendererData.Result), rendererData.Result));
            }
            catch (OperationCanceledException)
            {
                return OperationResult<ReportResult<TRendererData>>.Cancelled();
            }
            catch (Exception ex)
            {
                return OperationResult<ReportResult<TRendererData>>.InternalError("Report Error", ex.Message);
            }
        }

        protected async Task<BaseRendererData?> GetBaseRendererData(long userId, CancellationToken cancellationToken)
        {
            var reportContext = await _reportContextService.GetContextAsync(userId, cancellationToken);

            if (reportContext == null)
                return null;

            byte[]? logoBytes = null;
            if (!string.IsNullOrEmpty(reportContext.TenantLogo))
            {
                var cacheKey = $"TenantLogo:{reportContext.TenantLogo}";
                if (!_cache.TryGetValue(cacheKey, out logoBytes))
                {
                    var url = _filesStorage.GenerateSignedUrl(reportContext.TenantLogo);
                    if (!string.IsNullOrEmpty(url))
                    {
                        logoBytes = await UrlHelper.FetchImageBytesAsync(url);

                        if (logoBytes?.Length > 0)
                        {
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromHours(4))
                                .SetAbsoluteExpiration(TimeSpan.FromHours(24))
                                .SetPriority(CacheItemPriority.Low);

                            _cache.Set(cacheKey, logoBytes, cacheEntryOptions);
                        }
                    }
                }
            }

            return new BaseRendererData(reportContext, logoBytes);
        }

        protected abstract Task<OperationResult<TRendererData>> ExecuteReportLogic(long tenantId, long userId, ReportGenerationContext context, TRequest request);
    }
}
