using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AntActor.Core
{
    public interface IAnt { }

    public abstract class AbstractAnt<T> : IAnt
    {
        private readonly MailBox<AntMessage<T>> _mailBox;

        public AbstractAnt()
        {
            _mailBox = new MailBox<AntMessage<T>>();

            Task.Factory.StartNew(async () =>
            {
                await OnActivateAsync();
                while (true)
                {
                    var message = await _mailBox.ReceiveAsync();

                    try
                    {
                        await Handle(message);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }, TaskCreationOptions.LongRunning);

        }

        protected virtual Task OnActivateAsync() => Task.CompletedTask;
        protected abstract Task HandleMessage(T message);
        protected abstract Task<HandleResult> HandleError(AntMessage<T> message, Exception ex);

        protected void Post(T message)
        {
            _mailBox.Post(new AntMessage<T>(message));
        }

        protected Task<TResponse> PostAndReply<TResponse>(Func<ReplyChanel<TResponse>, T> messageFactory)
        {
            var res =  _mailBox.PostAndReplyAsync<TResponse>(rc => new AntMessage<T>(messageFactory(rc)));
            return res;
        }

        protected Task<TResponse> PostAndReply<TResponse>(Func<ReplyChanel<TResponse>, T> messageFactory, TimeSpan timeout)
        {
            var res = _mailBox.PostAndReplyAsync<TResponse>(rc => new AntMessage<T>(messageFactory(rc)), timeout);
            return res;
        }

        protected void ClearQueue() => _mailBox.Clear();

        private async Task Handle(AntMessage<T> message)
        {
            try
            {
                await HandleMessage(message.Message);
            }
            catch(Exception e)
            {
                message.OnError(e);
                var result = await HandleError(message, e);

                switch(result)
                {
                    case OkHandleResult x:
                        break;

                    default: throw new NotImplementedException("Not implemented handler for result");
                }
            }
        }
    }
}