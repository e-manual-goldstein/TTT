using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public class GameState
    {
        public GameState(Guid currentPlayerId, Cell[,] cells)
        {
            CurrentPlayerId = currentPlayerId;
            Cells = cells;
        }

        public Guid CurrentPlayerId { get; private set; }

        public Cell[,] Cells { get; private set; }
    }
}
