using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntActor.Chat.Backend.Ants.Models;
using AntActor.Chat.Backend.DAL;
using AntActor.Core;

namespace AntActor.Chat.Backend.Ants
{
    public interface IChannelAction {}

    public class GetMembersAction : IChannelAction
    {
        public ReplyChanel<string[]> ReplyChanel { get; }

        public GetMembersAction(ReplyChanel<string[]> rc) => ReplyChanel = rc;
    }

    public class JoinAction: IChannelAction
    {
        public string Nickname { get; }
        public ReplyChanel<Message> ReplyChanel { get; }

        public JoinAction(string nickname, ReplyChanel<Message> replyChanel)
        {
            Nickname = nickname;
            ReplyChanel = replyChanel;
        }
    }

    public class LeaveAction: IChannelAction
    {
        public string Nickname { get; }
        public ReplyChanel<Message> ReplyChanel { get; }

        public LeaveAction(string nickname, ReplyChanel<Message> replyChanel)
        {
            Nickname = nickname;
            ReplyChanel = replyChanel;
        }
    }

    public class SendAction: IChannelAction
    {
        public Message Message { get; }
        public ReplyChanel<Message> ReplyChanel { get; }

        public SendAction(Message message, ReplyChanel<Message> replyChanel)
        {
            Message = message;
            ReplyChanel = replyChanel;
        }
    }

    public class ReadHistoryAction: IChannelAction
    {
        public ReplyChanel<Message[]> ReplyChanel { get; }

        public ReadHistoryAction(ReplyChanel<Message[]> replyChanel)
        {
            ReplyChanel = replyChanel;
        }
    }

    public class NewMessageEventArgs: EventArgs
    {
        public Message Message { get; set; }
    }

public class ChannelAnt : AbstractAnt<IChannelAction>
{
    private readonly MessageRepository _messageRepository;
    private readonly string _channelName;
    private readonly List<string> _onlineMembers = new();
    private readonly List<Message> _messages = new();

    public ChannelAnt(MessageRepository messageRepository, string channelName)
    {
        _messageRepository = messageRepository;
        _channelName = channelName;
    }

    public async Task<string[]> GetMembers() => await PostAndReply<string[]>(rc => new GetMembersAction(rc));
    public async Task<Message> Join(string nickname) => await PostAndReply<Message>(rc => new JoinAction(nickname, rc));
    public async Task<Message> Leave(string nickname) => await PostAndReply<Message>(rc => new LeaveAction(nickname, rc));
    public async Task<Message> Send(Message message) => await PostAndReply<Message>(rc => new SendAction(message, rc));
    public async Task<Message[]> ReadHistory() => await PostAndReply<Message[]>(rc => new ReadHistoryAction(rc));

    protected override async Task OnActivateAsync()
    {
        var messages = await _messageRepository.GetAllForChannelAsync(_channelName);
        _messages.AddRange(messages);

        await base.OnActivateAsync();
    }

    protected override async Task HandleMessage(IChannelAction action)
    {
        Message message = null;
        switch (action)
        {
            case GetMembersAction a:
                a.ReplyChanel.Reply(_onlineMembers.ToArray());
                break;
            case JoinAction a:
                _onlineMembers.Add(a.Nickname);
                message = new Message($"{a.Nickname} joined the channel");
                _messages.Add(message);
                a.ReplyChanel.Reply(message);
                break;
            case LeaveAction a:
                _onlineMembers.Remove(a.Nickname);
                message = new Message($"{a.Nickname} left the channel");
                _messages.Add(message);
                a.ReplyChanel.Reply(message);
                break;
            case SendAction a:
                message = a.Message;
                _messages.Add(message);
                a.ReplyChanel.Reply(message);
                break;
            case ReadHistoryAction a:
                var messages = _messages
                    .OrderByDescending(x => x.Created)
                    .Reverse()
                    .ToArray();
                a.ReplyChanel.Reply(messages);
                break;
        }

        if(message is not null)
        {
            message.Channel = _channelName;
            await _messageRepository.AddAsync(message);
        }
    }

    protected override Task<HandleResult> HandleError(AntMessage<IChannelAction> message, Exception ex) => HandleResult.OkTask();
}
}