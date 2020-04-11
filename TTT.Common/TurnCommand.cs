using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public class TurnCommand : ISubCommand
    {
        public TurnCommand(Guid playerId, Cell cell)
        {
            PlayerId = playerId;
            Cell = cell;
        }

        public Guid PlayerId { get; set; }

        public Cell Cell { get; set; }

        public Type CommandType => GetType();
    }
}
