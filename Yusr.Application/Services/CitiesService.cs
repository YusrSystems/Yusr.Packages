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
    public class CitiesService : BaseApplicationService, ICitiesService
    {
        private readonly ICitiesRepository _cityRepo;
        public CitiesService(ILogger<CitiesService> logger, IExceptionService exceptionService, ICitiesRepository cityRepo) : base(logger, exceptionService)
        {
            _cityRepo = cityRepo;
        }

        protected override string EntityName => "المدينة";

        public async Task<OperationResult<FilterResponse<CityDto>>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _cityRepo.FilterAsync(pageNumber, rowsPerPage, condition);

                return OperationResult<FilterResponse<CityDto>>.Ok(new FilterResponse<CityDto>
                {
                    Data = result.Data.ToDtoList(),
                    Count = result.Count
                });
            },
            errorTitle: ErrorMessages.RetrievingError);
        }
    }
}
