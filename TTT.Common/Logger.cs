using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TTT.Common
{
    public class Logger
    {
        StackTrace _stackTrace;
        public Logger()
        {
            _stackTrace = new StackTrace();
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
