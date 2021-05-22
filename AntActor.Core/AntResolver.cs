using System;

namespace AntActor.Core
{
    public interface IAntResolver
    {
        T Resolve<T>(string id) where T : IAnt;
    }

    public class AntResolver: IAntResolver
    {
        public T Resolve<T>(string id) where T : IAnt 
            => (T)Activator.CreateInstance(typeof(T), id);
    }
}