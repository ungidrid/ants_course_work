using System;
using System.Threading.Tasks;
using AntActor.Chat.Backend.Ants;
using AntActor.Chat.Backend.Ants.Models;
using AntActor.Chat.Backend.DAL;
using AntActor.Core;
using Microsoft.AspNetCore.SignalR;

namespace AntActor.Chat.Backend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Anthill _anthill;
        private readonly MessageRepository _repository;

        public ChatHub(Anthill clusterClient, MessageRepository repository)
        {
            _anthill = clusterClient;
            _repository = repository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            Context.SetAccessToken(httpContext.Request.Headers["Authorization"].ToString().Split(" ")[1]);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var client = _anthill.GetAnt<ClientAnt>(Context.GetAccessToken());

            var activeChannel = client.ActiveChannel;
            var message = await client.LeaveChannel();
            await SendMessageNotification(activeChannel, message);

            var clientChannelMapping = _anthill.GetAnt<ClientChannelMappingAnt>(Constants.ClientChannelMappingAntId);
            clientChannelMapping.RemoveMapping(Context.ConnectionId, activeChannel);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetChannelHistory(string channelName)
        {
            var channel = _anthill.GetAnt<ChannelAnt>(channelName);
            var history = await channel.ReadHistory();
            await Clients.Client(Context.ConnectionId).SendAsync("send.history", channelName, history);
        }

        public async Task GetChannelMembers(string channelName)
        {
            var channel = _anthill.GetAnt<ChannelAnt>(channelName);
            var members = await channel.GetMembers();
            await Clients.Client(Context.ConnectionId).SendAsync("send.members", channelName, members);
        }

        public async Task JoinChannel(string channelName)
        {
            var client = _anthill.GetAnt<ClientAnt>(Context.GetAccessToken());

            var message = await client.JoinChannel(channelName);

            var clientChannelMapping = _anthill.GetAnt<ClientChannelMappingAnt>(Constants.ClientChannelMappingAntId);
            clientChannelMapping.AddMapping(Context.ConnectionId, channelName);

            await SendMessageNotification(channelName, message);
        }

        public async Task LeaveChannel(string channelName)
        {
            var client = _anthill.GetAnt<ClientAnt>(Context.GetAccessToken());

            var message = await client.LeaveChannel();

            await SendMessageNotification(channelName, message);

            var clientChannelMapping = _anthill.GetAnt<ClientChannelMappingAnt>(Constants.ClientChannelMappingAntId);
            clientChannelMapping.RemoveMapping(Context.ConnectionId, channelName);
        }

        public async Task SetUsername(string username)
        {
            var client = _anthill.GetAnt<ClientAnt>(Context.GetAccessToken());

            client.SetUsername(username);
        }

        public async Task SendMessage(string message)
        {
            var client = _anthill.GetAnt<ClientAnt>(Context.GetAccessToken());

            var responseMessage = await client.SendMessage(message);
            await SendMessageNotification(client.ActiveChannel, responseMessage);
        }

        private async Task SendMessageNotification(string channelName, Message message)
        {
            var mappings = _anthill.GetAnt<ClientChannelMappingAnt>(Constants.ClientChannelMappingAntId);

            var clientIds = await mappings.GetMAppingsForChannel(channelName);

            await Clients.Clients(clientIds)
                .SendAsync("send.message", channelName, message);
;        }
    }
}