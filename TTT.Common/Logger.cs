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
        Verbosity _verbosity = Verbosity.Warn;

        public Logger()
        {
#if DEBUG
            _verbosity = Verbosity.Debug;
#endif
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
            LogInMemory(message);
            if (MessageReceived != null)
                MessageReceived(message);
        }

        public void Debug(string message)
        {
            if (_verbosity >= Verbosity.Debug)
                Log($"[Debug] {message}");
        }

        public void Warning(string message) 
        {
            if (_verbosity >= Verbosity.Warn)
                Log($"[WARN] {message}");
        }

        public void Error(string message)
        {
            if (_verbosity >= Verbosity.Error)
            {
                Log($"[ERROR] {message}");
                Log($"{new StackTrace()}");
            }
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
    public enum Verbosity
    {
        Error = 0,
        Warn = 1,
        Log = 2,
        Debug = 3
    }
}
