using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class GameGrid
    {
        Cell[] _allCells;
        PlayerManager _playerManager;
        


        public GameGrid(ActionService actionService, PlayerManager playerManager)
        {
            _allCells = CreateCells(actionService);
            _playerManager = playerManager;
        }

        public Cell[] CreateCells(ActionService actionService)
        {
            var cells = new List<Cell>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var cell = new Cell(i, j);
                    cell.SetTakeTurnAction((c) => actionService.TakeTurn(c));
                    cells.Add(cell);
                }
            }
            return cells.ToArray();
        }

        public Cell[] AllCells => _allCells;

        public bool IsMyTurn(GameState gameState) 
        {
            return _playerManager.IsMyTurn(gameState);
        }
    }   
}