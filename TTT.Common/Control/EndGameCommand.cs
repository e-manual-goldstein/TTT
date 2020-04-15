using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public class EndGameCommand : ISubCommand
    {
        public EndGameCommand(Guid winner, Cell[] winningSet)
        {
            Winner = winner;
            WinningSet = winningSet;
        }

        public Guid Winner { get; private set; }

        public Cell[] WinningSet { get; private set; }
    }
}
