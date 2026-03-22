using Microsoft.Extensions.Caching.Memory;
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
using Yusr.Identity.Abstractions.Interfaces;
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Application.Services
{
    public class RolesService<TSystemPermissions> : BaseApplicationService, IRolesService where TSystemPermissions : ISystemPermissions
    {
        private readonly IRolesRepository _rolesRepo;
        private readonly IMemoryCache _cache;
        private readonly IRolePermissionService _rolePermissionService;

        public RolesService(
            IRolesRepository rolesRepo,
            ILogger<RolesService<TSystemPermissions>> logger,
            IExceptionService exceptionService,
            IMemoryCache cache,
            IRolePermissionService rolePermissionService
        ) : base(logger, exceptionService)
        {
            _rolesRepo = rolesRepo;
            _cache = cache;
            _rolePermissionService = rolePermissionService;
        }

        protected override string EntityName => "الدور";

        public async Task<OperationResult<FilterResponse<RoleDto>>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _rolesRepo.FilterAsync(pageNumber, rowsPerPage, condition);

                return OperationResult<FilterResponse<RoleDto>>.Ok(new FilterResponse<RoleDto>
                {
                    Data = result.Data.ToDtoList(),
                    Count = result.Count
                });
            },
            errorTitle: ErrorMessages.RetrievingError);
        }

        public async Task<OperationResult<RoleDto>> GetByIdAsync(long id)
        {
            return await ExecuteAsync(async () =>
            {
                var entity = await _rolesRepo.GetByIdAsync(id);
                if (entity == null)
                    return OperationResult<RoleDto>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                var dbPermissions = entity.Permissions ?? Enumerable.Empty<string>();

                var validPermissions = dbPermissions
                    .Where(p => TSystemPermissions.All.Contains(p))
                    .ToList();

                entity.Update(entity.Name, validPermissions);

                return OperationResult<RoleDto>.Ok(entity.ToDto());
            },
            errorTitle: ErrorMessages.RetrievingError);
        }

        public async Task<OperationResult<RoleDto>> AddAsync(RoleDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                var sanitizedPermissions = dto.Permissions
                    .Where(p => TSystemPermissions.All.Contains(p))
                    .Distinct()
                    .ToList();

                var role = Role.Create(dto.Name, sanitizedPermissions);

                await _rolesRepo.AddAsync(role);
                await _rolesRepo.SaveChangesAsync();

                return OperationResult<RoleDto>.Ok(role.ToDto());
            },
            errorTitle: ErrorMessages.FailedToAdd(EntityName));
        }

        public async Task<OperationResult<RoleDto>> UpdateAsync(RoleDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                var role = await _rolesRepo.GetByIdAsync(dto.Id, track: true);

                if (role == null)
                    return OperationResult<RoleDto>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                var sanitizedPermissions = dto.Permissions
                    .Where(p => TSystemPermissions.All.Contains(p))
                    .Distinct()
                    .ToList();

                role.Update(dto.Name, sanitizedPermissions);

                await _rolesRepo.UpdateAsync(role);
                await _rolesRepo.SaveChangesAsync();

                _cache.Remove(_rolePermissionService.BuildCacheKey(role.Id));

                return OperationResult<RoleDto>.Ok(role.ToDto());
            },
            errorTitle: ErrorMessages.FailedToUpdate(EntityName));
        }

        public async Task<OperationResult<bool>> DeleteAsync(long id)
        {
            return await ExecuteAsync(async () =>
            {
                int rows = await _rolesRepo.DeleteAsync(id);

                if (rows == 0)
                    return OperationResult<bool>.NotFound(ErrorMessages.EntityNotFound(EntityName));

                _cache.Remove(_rolePermissionService.BuildCacheKey(id));

                return OperationResult<bool>.Ok(true);
            },
           errorTitle: ErrorMessages.FailedToDelete(EntityName));
        }
    }
}
