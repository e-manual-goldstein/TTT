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

            _verbosity = Verbosity.Debug;

        }

        public void Log(string message)
        {
            Console.WriteLine(message);
            LogInMemory(message);
            if (MessageReceived != null)
            {
                try
                {
                    MessageReceived(message);
                }
                catch (Exception ex)
                {
                    LogInMemory("Error exeucting Message Received handler");
                    LogInMemory(ex.Message);
                    LogInMemory(ex.StackTrace);
                }
            }
        }

        public void Debug(string message)
        {
            if (_verbosity >= Verbosity.Debug)
                Log($"[DEBUG] {message}");
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

        public void Error(Exception exception)
        {
            if (_verbosity >= Verbosity.Error)
            {
                Log($"[ERROR] {exception.Message}");
                Log($"{exception.StackTrace}");
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
        Silent = -1,
        Error = 0,
        Warn = 1,
        Debug = 2
    }
}
