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
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Application.Services
{
    public class UsersService : BaseApplicationService, IUsersService
    {
        private readonly IUsersRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public UsersService(
            IUnitOfWork unitOfWork,
            IUsersRepository userRepo,
            IPasswordService passwordService,
            ILogger<UsersService> logger, IExceptionService exceptionService) : base(logger, exceptionService)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        protected override string EntityName => "المستخدم";

        public async Task<OperationResult<FilterResponse<UserDto>>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _userRepo.FilterAsync(pageNumber, rowsPerPage, condition);

                return OperationResult<FilterResponse<UserDto>>.Ok(new FilterResponse<UserDto>
                {
                    Data = result.Data.ToDtoList(),
                    Count = result.Count
                });
            },
            errorTitle: ErrorMessages.RetrievingError);
        }

        public async Task<OperationResult<UserDto>> GetByIdAsync(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var entity = await _userRepo.GetByIdAsync(id);
                if (entity == null)
                    return OperationResult<UserDto>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                return OperationResult<UserDto>.Ok(entity.ToDto());
            },
            errorTitle: ErrorMessages.RetrievingError);
        }

        public async Task<OperationResult<UserDto>> AddAsync(UserDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                var user = User.Create(dto.Username, dto.IsActive, dto.BranchId, dto.RoleId);
                var hashedPassword = _passwordService.Hash(user, dto.Password);
                user.ChangePassword(hashedPassword);

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                var newUser = await _userRepo.GetByIdAsync(user.Id);
                return OperationResult<UserDto>.Ok(newUser!.ToDto());
            },
            errorTitle: ErrorMessages.FailedToAdd(EntityName));
        }

        public async Task<OperationResult<UserDto>> UpdateAsync(UserDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();

                var user = await _userRepo.GetByIdAsync(dto.Id, track: true);
                if (user == null)
                    return OperationResult<UserDto>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                var hashedPassword = _passwordService.Hash(user, dto.Password);
                user
                    .Update(dto.Username, dto.IsActive, dto.BranchId, dto.RoleId)
                    .ChangePassword(hashedPassword);

                await _userRepo.UpdateAsync(user);
                await _userRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                var newUser = await _userRepo.GetByIdAsync(user.Id);
                return OperationResult<UserDto>.Ok(newUser!.ToDto());
            },
            errorTitle: ErrorMessages.FailedToUpdate(EntityName));
        }

        public async Task<OperationResult<bool>> DeleteAsync(long id)
        {
            return await ExecuteAsync(async () =>
            {
                int rows = await _userRepo.DeleteAsync(id);

                if (rows == 0)
                    return OperationResult<bool>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                return OperationResult<bool>.Ok(true);
            },
           errorTitle: ErrorMessages.FailedToDelete(EntityName));
        }
    }
}
