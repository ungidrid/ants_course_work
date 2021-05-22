using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AntActor.Core
{
    public class MailBox<T>
    {
        private readonly BufferBlock<T> _mailBox;

        public MailBox()
        {
            _mailBox = new BufferBlock<T>();
        }

        public void Post(T msg) => _mailBox.Post(msg);
        public void Clear() => _mailBox.TryReceiveAll(out _);

        public Task<T> ReceiveAsync() => _mailBox.ReceiveAsync();

        public Task<TResponse> PostAndReplyAsync<TResponse>(Func<ReplyChanel<TResponse>, T> messageFactory)
        {
            var replyChanel = new ReplyChanel<TResponse>();
            var message = messageFactory(replyChanel);

            Post(message);

            return replyChanel.GetReply;
        }

        public async Task<TResponse> PostAndReplyAsync<TResponse>(Func<ReplyChanel<TResponse>, T> messageFactory, TimeSpan timeout)
        {
            var postTask = PostAndReplyAsync(messageFactory);
            var delayTask = Task.Delay(timeout);
            var res = await Task.WhenAny(postTask, delayTask);

            return res == delayTask ? default : postTask.Result;
        }
    }
}