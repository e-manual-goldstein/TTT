using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Host
{
    public class InvalidPortException : Exception
    {
        public InvalidPortException(string message) : base(message)
        {

        }
    }
}
