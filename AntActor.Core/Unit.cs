using System.Threading.Tasks;

namespace AntActor.Core
{
    public class Unit
    {
        public static Unit Value { get; } = new();
        public static Task<Unit> Task { get; } = System.Threading.Tasks.Task.FromResult(Value);
    }
}