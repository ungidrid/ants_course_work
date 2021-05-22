using System.Threading.Tasks;

namespace AntActor.Core
{
    public class ReplyChanel<T>
    {
        private readonly TaskCompletionSource<T> _taskCompletionSource;

        public Task<T> GetReply => _taskCompletionSource.Task;

        public ReplyChanel()
        {
            _taskCompletionSource = new TaskCompletionSource<T>();
        }

        public void Reply(T reply)
        {
            _taskCompletionSource.SetResult(reply);
        }
    }
}