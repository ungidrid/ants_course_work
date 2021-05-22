using System;
using Microsoft.Extensions.DependencyInjection;

namespace AntActor.Core
{
    public class DIResolver: IAntResolver
    {
        private readonly IServiceProvider _provider;

        public DIResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T Resolve<T>(string id) where T : IAnt
        {
            return (T)ActivatorUtilities.CreateInstance(_provider, typeof(T), id);
        }
    }
}