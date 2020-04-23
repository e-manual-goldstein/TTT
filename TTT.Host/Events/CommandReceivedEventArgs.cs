using System;
using System.Collections.Generic;
using System.Text;
using TTT.Common;

namespace TTT.Host
{
    public class CommandReceivedEventArgs : EventArgs
    {
        public CommandReceivedEventArgs(GameCommand gameCommand)
        {
            Command = gameCommand;
        }

        public GameCommand Command { get; set; }
    }
}
