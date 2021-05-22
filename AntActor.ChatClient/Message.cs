using System;
using System.Text.Json.Serialization;

namespace AntActor.ChatClient
{
    public class Message
    {
        [JsonPropertyName("created")]
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;

        [JsonPropertyName("author")]
        public string Author { get; set; } = "Server";

        [JsonPropertyName("text")]
        public string Text { get; set; }

        public string GetMessageString()
        {
            return $"[{Created.DateTime.ToShortTimeString()} {Author}]: {Text}";
        }
    }
}