using Microsoft.Extensions.DependencyInjection;

namespace AntActor.Core
{
    public static class Configure
    {
        public static void AddAntActor(this IServiceCollection services)
        {
            services.AddTransient<IAntResolver, DIResolver>();
            services.AddSingleton<Anthill>();
        }
    }
}