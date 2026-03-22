using Yusr.Core.Abstractions.Primitives;

namespace Yusr.Application.Abstractions.Interfaces.Generics
{
    public interface IDeletableService
    {
        public Task<OperationResult<bool>> DeleteAsync(long id);
    }
}
