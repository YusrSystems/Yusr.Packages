using Microsoft.Extensions.Logging;
using Yusr.Application.Abstractions.DTOs;
using Yusr.Application.Abstractions.Interfaces;
using Yusr.Application.Abstractions.Mappings;
using Yusr.Application.Abstractions.Services;
using Yusr.Core.Abstractions.Constants;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Core.Abstractions.Services;

namespace Yusr.Application.Services
{
    public class BranchesService : BaseApplicationService, IBranchesService
    {
        private readonly IBranchesRepository _branchRepo;
        public BranchesService(ILogger<BranchesService> logger, IExceptionService exceptionService, IBranchesRepository branchRepo, IUnitOfWork unitOfWork) : base(logger, exceptionService)
        {
            _branchRepo = branchRepo;
        }



        public async Task<OperationResult<FilterResponse<BranchDto>>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _branchRepo.FilterAsync(pageNumber, rowsPerPage, condition);

                return OperationResult<FilterResponse<BranchDto>>.Ok(new FilterResponse<BranchDto>
                {
                    Data = result.Data.ToDtoList(),
                    Count = result.Count
                });
            },
            errorTitle: ErrorMessages.RetrievingError);
        }
        public async Task<OperationResult<BranchDto>> GetByIdAsync(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var entity = await _branchRepo.GetByIdAsync(id);
                if (entity == null)
                    return OperationResult<BranchDto>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                return OperationResult<BranchDto>.Ok(entity.ToDto());
            },
            errorTitle: ErrorMessages.RetrievingError);
        }
        public async Task<OperationResult<BranchDto>> AddAsync(BranchDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                var branch = Branch.Create(dto.Name, dto.CityId, dto.Street, dto.District, dto.BuildingNumber, dto.PostalCode);

                await _branchRepo.AddAsync(branch);
                await _branchRepo.SaveChangesAsync();

                var newBranch = await _branchRepo.GetByIdAsync(branch.Id);
                return OperationResult<BranchDto>.Ok(newBranch!.ToDto());
            },
            errorTitle: ErrorMessages.FailedToAdd(EntityName));
        }
        public async Task<OperationResult<BranchDto>> UpdateAsync(BranchDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                var branch = await _branchRepo.GetByIdAsync(dto.Id, track: true);
                if (branch == null)
                    return OperationResult<BranchDto>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                branch.Update(dto.Name, dto.CityId, dto.Street, dto.District, dto.BuildingNumber, dto.PostalCode);

                await _branchRepo.UpdateAsync(branch);
                await _branchRepo.SaveChangesAsync();

                var newBranch = await _branchRepo.GetByIdAsync(branch.Id);
                return OperationResult<BranchDto>.Ok(newBranch!.ToDto());
            },
            errorTitle: ErrorMessages.FailedToUpdate(EntityName));
        }
        public async Task<OperationResult<bool>> DeleteAsync(long id)
        {
            return await ExecuteAsync(async () =>
            {
                int rows = await _branchRepo.DeleteAsync(id);

                if (rows == 0)
                    return OperationResult<bool>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                return OperationResult<bool>.Ok(true);
            },
            errorTitle: ErrorMessages.FailedToDelete(EntityName));
        }
    }
}
