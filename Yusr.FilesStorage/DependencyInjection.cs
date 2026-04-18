using Microsoft.Extensions.DependencyInjection;
using Yusr.Storage.Abstractions.Services;
using Yusr.Storage.Providers;

namespace Yusr.Storage
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYusrFilesStorage(this IServiceCollection services)
        {
            services.AddSingleton<IFilesStorage, S3Service>();

            return services;
        }
    }
}
