using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TTT.Common
{
    public class Logger
    {
        string[] _inMemoryLog = new string[] { };

        public Logger()
        {
            
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
            LogInMemory(message);
            if (MessageReceived != null)
                MessageReceived(message);
        }

        public void Warning(string message) 
        {
            Console.WriteLine($"[WARN] {message}");
            LogInMemory(message);
            if (MessageReceived != null)
                MessageReceived($"[WARN] {message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
            LogInMemory(message);
            var stackTrace = new StackTrace();
            Console.WriteLine(stackTrace);
            if (MessageReceived != null)
                MessageReceived($"[ERROR] {message}");
        }

        public event MessageReceivedEvent MessageReceived;
        public delegate void MessageReceivedEvent(string message);

        public void LogInMemory(string message)
        {
            _inMemoryLog = _inMemoryLog.Concat(new string[] { message }).ToArray();
        }

        public string ReadFromLog(int? backlog = null)
        {
            var sb = new StringBuilder();
            var buffer = backlog == null ? _inMemoryLog : _inMemoryLog.Skip(_inMemoryLog.Length - backlog.Value);
            foreach (var line in buffer)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}
