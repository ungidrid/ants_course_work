using System;

namespace AntActor.Core
{
    public class AntMessage<T>
    {
        public T Message { get; set; }
        public int Attempt { get; private set; }
        public DateTime EnterTime { get; set; }
        public Exception LastError { get; private set; }

        public AntMessage(T message)
        {
            Message = message;
            Attempt = 0;
            EnterTime = DateTime.Now;
            LastError = null;
        }

        public void OnError(Exception ex)
        {
            LastError = ex;
            Attempt++;
        }
    }
}