using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTT
{
    public static class Output
    {
        public static Action<string> OutputMode { get; set; }
        public static void Test(string message)
        {

        }

        public static void Debug(string message)
        {
            if (OutputMode != null)
                OutputMode(message);
            Console.WriteLine(message);
        }

        public static void Debug(Func<string> messageFunc)
        {
            if (Runtime.Debug)
            {
                Console.WriteLine(messageFunc());
            }
            if (OutputMode != null)
                OutputMode(messageFunc());
        }

        public static void Debug(List<string> messages)
        {
            messages.ForEach(m => Debug(() => m));
        }

        public static void WriteLine(List<string> messages)
        {

        }
    }
}
