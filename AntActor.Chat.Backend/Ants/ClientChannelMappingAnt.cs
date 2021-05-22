using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntActor.Core;

namespace AntActor.Chat.Backend.Ants
{
    public interface IClientChannelMappingAction
    {
    }

    public class AddMapping: IClientChannelMappingAction
    {
        public string ClientId { get; set; }
        public string ChannelName { get; set; }

        public AddMapping(string clientId, string channelName)
        {
            ClientId = clientId;
            ChannelName = channelName;
        }
    }

    public class GetMappingsForChannel: IClientChannelMappingAction
    {
        public string ChannelName { get; }
        public ReplyChanel<string[]> ReplyChanel { get; }

        public GetMappingsForChannel(string channelName, ReplyChanel<string[]> replyChanel)
        {
            ChannelName = channelName;
            ReplyChanel = replyChanel;
        }
    }

    public class RemoveMapping: IClientChannelMappingAction
    {
        public string ClientId { get; }
        public string ChannelName { get; }

        public RemoveMapping(string clientId, string channelName)
        {
            ClientId = clientId;
            ChannelName = channelName;
        }
    }

    public class ClientChannelMappingAnt : AbstractAnt<IClientChannelMappingAction>
    {
        private Dictionary<string, List<string>> _clientChannelMappings = new();

        public void AddMapping(string clientId, string channelName) => Post(new AddMapping(clientId, channelName));

        public async Task<string[]> GetMAppingsForChannel(string channelName) =>
            await PostAndReply<string[]>(rc => new GetMappingsForChannel(channelName, rc));

        public void RemoveMapping(string clientId, string channelName) => Post(new RemoveMapping(clientId, channelName));

        public ClientChannelMappingAnt(string id)
        {
        }

        protected override Task HandleMessage(IClientChannelMappingAction message)
        {
            switch(message)
            {
                case AddMapping a:
                    if(!_clientChannelMappings.TryGetValue(a.ChannelName, out _))
                        _clientChannelMappings.Add(a.ChannelName, new[] { a.ClientId }.ToList());
                    else
                        _clientChannelMappings[a.ChannelName].Add(a.ClientId);;
                    break;
                case GetMappingsForChannel a:
                    a.ReplyChanel.Reply(_clientChannelMappings[a.ChannelName]?.ToArray());
                    break;
                case RemoveMapping a:
                    if(_clientChannelMappings.TryGetValue(a.ChannelName, out List<string> value))
                        value.Remove(a.ClientId);

                    break;
            }

            return Task.CompletedTask;
        }

        protected override Task<HandleResult> HandleError(AntMessage<IClientChannelMappingAction> message, Exception ex) => null;
    }
}