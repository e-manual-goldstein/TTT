using System;
using System.Collections.Generic;
using System.Text;
using TTT.Common;

namespace TTT.Host.Events
{
    public class EndGameEventArgs : EventArgs
    {
        public Guid Winner { get; set; }

        public Cell[] WinningSet { get; set; }
    }
}
