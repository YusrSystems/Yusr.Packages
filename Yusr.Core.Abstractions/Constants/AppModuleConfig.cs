using Yusr.Core.Abstractions.Enums;

namespace Yusr.Core.Abstractions.Constants
{
    public static class AppModuleConfig
    {
        private static AppModule? _module;

        public static AppModule Module
            => _module ?? throw new InvalidOperationException("AppModuleConfig not initialized.");

        public static void Init(AppModule type)
        {
            if (_module.HasValue)
                throw new InvalidOperationException("AppModuleConfig already initialized.");

            _module = type;
        }
    }
}
