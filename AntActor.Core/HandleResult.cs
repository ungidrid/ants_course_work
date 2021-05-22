using System.Threading.Tasks;

namespace AntActor.Core
{
    public class HandleResult
    {
        public static HandleResult Ok() => OkHandleResult.Instance;
        public static Task<HandleResult> OkTask() => OkHandleResult.Task;
    }

    internal class OkHandleResult: HandleResult
    {
        internal static readonly OkHandleResult Instance = new();
        internal static readonly Task<HandleResult> Task = System.Threading.Tasks.Task.FromResult((HandleResult)Instance);

        private OkHandleResult() { }
    }
}