using System;
using System.Threading.Tasks;
using AntActor.Chat.Backend.Ants.Models;
using AntActor.Core;

namespace AntActor.Chat.Backend.Ants
{
    public interface IClientAction { }

    public class ClientJoinAction: IClientAction
    {
        public string ChannelName { get; }
        public ReplyChanel<Message> ReplyChanel { get; }

        public ClientJoinAction(string channelName, ReplyChanel<Message> replyChanel)
        {
            ChannelName = channelName;
            ReplyChanel = replyChanel;
        }
    }

    public class ClientLeaveAction: IClientAction
    {
        public ReplyChanel<Message> ReplyChanel { get; }

        public ClientLeaveAction(ReplyChanel<Message> replyChanel)
        {
            ReplyChanel = replyChanel;
        }
    }

    public class ClientSetUsernameAction: IClientAction
    {
        public string Username { get; }

        public ClientSetUsernameAction(string username)
        {
            Username = username;
        }
    }
    
    public class ClientSendMessageAction: IClientAction
    {
        public string Message { get; }
        public ReplyChanel<Message> ReplyChanel { get; }

        public ClientSendMessageAction(string message, ReplyChanel<Message> replyChanel)
        {
            Message = message;
            ReplyChanel = replyChanel;
        }
    }

    public class ClientAnt: AbstractAnt<IClientAction>
    {
        private readonly Anthill _anthill;
        public string ActiveChannel { get; private set; } = "";
        private string _userName = "";

        public ClientAnt(Anthill anthill, string id)
        {
            _anthill = anthill;
        }

        public Task<Message> JoinChannel(string channelName) => PostAndReply<Message>(rc => new ClientJoinAction(channelName, rc));
        public Task<Message> LeaveChannel() => PostAndReply<Message>(rc => new ClientLeaveAction(rc));
        public void SetUsername(string username) => Post(new ClientSetUsernameAction(username));
        public Task<Message> SendMessage(string message) => PostAndReply<Message>(rc=>new ClientSendMessageAction(message,rc));

        protected override async Task HandleMessage(IClientAction action)
        {
            ChannelAnt channel = null;
            Message message = null;
            switch(action)
            {
                case ClientJoinAction a:
                {
                    channel = _anthill.GetAnt<ChannelAnt>(a.ChannelName);
                    message = await channel.Join(_userName);
                    a.ReplyChanel.Reply(message);
                    ActiveChannel = a.ChannelName;
                    break;
                }
                case ClientLeaveAction a:
                {
                    channel = _anthill.GetAnt<ChannelAnt>(ActiveChannel);
                    message = await channel.Leave(_userName);
                    ActiveChannel = "";
                    a.ReplyChanel.Reply(message);
                    break;
                }
                case ClientSetUsernameAction a:
                {
                    _userName = a.Username;
                    break;
                }
                case ClientSendMessageAction a:
                {
                    channel = _anthill.GetAnt<ChannelAnt>(ActiveChannel);
                    message = await channel.Send(new Message(_userName, a.Message));
                    a.ReplyChanel.Reply(message);
                    break;
                }
            }
        }

        protected override Task<HandleResult> HandleError(AntMessage<IClientAction> message, Exception ex) => HandleResult.OkTask();
    }
}