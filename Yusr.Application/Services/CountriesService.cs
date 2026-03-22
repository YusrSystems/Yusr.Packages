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

    public class CountriesService : BaseApplicationService, ICountriesService
    {
        private readonly ICountriesRepository _countryRepo;
        public CountriesService(ILogger<CountriesService> logger, IExceptionService exceptionService, ICountriesRepository countryRepo) : base(logger, exceptionService)
        {
            _countryRepo = countryRepo;
        }

        protected override string EntityName => "الدولة";

        public async Task<OperationResult<FilterResponse<CountryDto>>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _countryRepo.FilterAsync(pageNumber, rowsPerPage, condition);

                return OperationResult<FilterResponse<CountryDto>>.Ok(new FilterResponse<CountryDto>
                {
                    Data = result.Data.ToDtoList(),
                    Count = result.Count
                });
            },
            errorTitle: ErrorMessages.RetrievingError); ;
        }
    }
}
