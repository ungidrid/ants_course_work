using System;
using System.Collections.Concurrent;

namespace AntActor.Core
{
    public class Anthill
    {
        private readonly IAntResolver _antResolver;
        private readonly ConcurrentDictionary<(Type, string), IAnt> _ants = new();

        public Anthill(IAntResolver antResolver)
        {
            _antResolver = antResolver;
        }

        public void RegisterAnt<T>(string id, T ant) where T: IAnt
        {
            _ants.AddOrUpdate((typeof(T), id), ant, (tuple, ant1) => ant1);
        }

        public T GetAnt<T>(string id) where T: IAnt
        {
            return (T)_ants.GetOrAdd((typeof(T), id), _ => _antResolver.Resolve<T>(id));
        }
    }
}