using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace AntActor.Chat.Backend.Ants.Models
{
    public class Message
    {
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public string Author { get; set; } = "Server";
        public string Text { get; set; }
        public string Channel { get; set; }

        public Message()
        {
            
        }
        public Message(string message)
        {
            Text = message;
        }

        public Message(string author, string message)
        {
            Author = author;
            Text = message;
        }
    }
}