using System.Text.Json;

namespace AntActor.ChatClient
{
    public static class Extensions
    {
        public static T ToOjbect<T>(this JsonElement element)
        {
            var rawText = element.GetRawText();
            return JsonSerializer.Deserialize<T>(rawText);
        }
    }
}