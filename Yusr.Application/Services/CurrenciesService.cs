using Microsoft.Extensions.Logging;
using Yusr.Application.Abstractions.DTOs;
using Yusr.Application.Abstractions.Interfaces;
using Yusr.Application.Abstractions.Mappings;
using Yusr.Application.Abstractions.Services;
using Yusr.Core.Abstractions.Constants;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Core.Abstractions.Services;

namespace Yusr.Application.Services
{
    public class CurrenciesService : BaseApplicationService, ICurrenciesService
    {
        private readonly ICurrenciesRepository _currenciesRepo;
        public CurrenciesService(ILogger<CurrenciesService> logger, IExceptionService exceptionService, ICurrenciesRepository currenciesRepo) : base(logger, exceptionService)
        {
            _currenciesRepo = currenciesRepo;
        }

        protected override string EntityName => "العملة";

        public async Task<OperationResult<FilterResponse<CurrencyDto>>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _currenciesRepo.FilterAsync(pageNumber, rowsPerPage, condition);

                return OperationResult<FilterResponse<CurrencyDto>>.Ok(new FilterResponse<CurrencyDto>
                {
                    Data = result.Data.ToDtoList(),
                    Count = result.Count
                });
            },
            errorTitle: ErrorMessages.RetrievingError);
        }
    }
}
