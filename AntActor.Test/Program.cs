using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AntActor.Core;
using AntActor.Test;

var anthill = new Anthill(new AntResolver());

void WriteToFile()
{
    var ant = anthill.GetAnt<FileAnt>("data.txt");
    for(var i = 0; i < 100; i++)
    {
        string calculationResult = null;
    #region Complex calculations
        calculationResult = $"[{Thread.CurrentThread.ManagedThreadId}]: {i}";
#endregion
        
        ant.AppendText(calculationResult);

    #region More complex calculations
        
#endregion
    }
};

var tasks = Enumerable.Range(0, 100)
    .Select(_ => Task.Run(WriteToFile));

await Task.WhenAll(tasks);

namespace AntActor.Test {
    class AppendTextMessage
    {
        public string Text { get; }

        public AppendTextMessage(string text)
        {
            Text = text;
        }
    }

    class FileAnt : AbstractAnt<AppendTextMessage>
    {
        private readonly string _fileName;

        public FileAnt(string fileName)
        {
            _fileName = fileName;
        }

        public void AppendText(string text) => Post(new AppendTextMessage(text));

        protected override Task HandleMessage(AppendTextMessage message)
        {
            File.AppendAllText(_fileName, message.Text);

            return Task.CompletedTask;
        }

        protected override Task<HandleResult> HandleError(
            AntMessage<AppendTextMessage> message, Exception ex) => null;
    }
}