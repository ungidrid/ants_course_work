using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;

namespace AntActor.ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HubConnection _connection;

        public MainWindow()
        {
            InitializeComponent();

            _connection = new HubConnectionBuilder().WithUrl(
                    "https://localhost:44308/chat-hub",
                    options => options.AccessTokenProvider = () => Task.FromResult(
                                   Guid.NewGuid()
                                       .ToString()))
                .Build();

            _connection.On<string, JsonElement>(
                "send.message",
                (s, o) =>
                {
                    var message = o.ToOjbect<Message>();
                    ChatAreaTextBlock.Text = ChatAreaTextBlock.Text + "\n" + message.GetMessageString();
                });

            _connection.On<string, JsonElement>(
                "send.history",
                (s, o) =>
                {
                    var messages = o.ToOjbect<Message[]>();
                    var history = messages.Aggregate("", (res, message) => res += message.GetMessageString() + "\n");
                    history = history.TrimEnd('\n');
                    if(!string.IsNullOrWhiteSpace(history))
                        ChatAreaTextBlock.Text = history;
                });

            _connection.StartAsync();
        }


        private async void ToggleChannelButton_Click(object sender, RoutedEventArgs e)
        {
            if(ChannelNameInput.IsReadOnly)
            {
                await _connection.InvokeAsync("LeaveChannel", ChannelNameInput.Text);
                ChannelNameInput.IsReadOnly = false;
                ChatAreaTextBlock.Text = "";
                ToggleChannelButton.Content = "Join";
            }
            else
            {
                await _connection.InvokeAsync("JoinChannel", ChannelNameInput.Text);
                await _connection.InvokeAsync("GetChannelHistory", ChannelNameInput.Text);
                ChannelNameInput.IsReadOnly = true;
                ToggleChannelButton.Content = "Leave";
            }
        }

        private async void SetUsernameButton_OnClick(object sender, RoutedEventArgs e)
        {
            await _connection.InvokeAsync("SetUsername", UsernameInput.Text);
        }

        private async void SendMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            await _connection.InvokeAsync("SendMessage", MessageInput.Text);
            MessageInput.Text = "";
        }

        private async void MessageInput_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.Enter)
                return;

            await _connection.InvokeAsync("SendMessage", MessageInput.Text);
            MessageInput.Text = "";
        }
    }
}