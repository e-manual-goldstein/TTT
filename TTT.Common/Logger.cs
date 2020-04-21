using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TTT.Common
{
    public class Logger
    {
        //LogLevel

        public Logger()
        {
            
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Warning(string message) 
        {
            Console.WriteLine($"[WARN] {message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
            var stackTrace = new StackTrace();
            Console.WriteLine(stackTrace);
        }
    }
}
