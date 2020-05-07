using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TTT.Common;
using TTT.Host;
using System.Linq;
using TTT.Host.Events;

namespace TTT.Host
{
    public class Game //: IUpdatingElement
    {
        Guid _gameId;
        Cell[] _cellGrid;
        List<Cell> _allCells = new List<Cell>();
        Cell[][] _sets;
       
        Dictionary<Guid, Marker> _players = new Dictionary<Guid, Marker>();
        Guid _currentPlayerId;

        EventHandler _turnTaken;
        EventHandler<EndGameEventArgs> _endGame;

        public Game(Guid gameId, EventHandler turnTakenEventHandler, EventHandler<EndGameEventArgs> endGameEventHandler)
        {
            _gameId = gameId;
            _turnTaken += turnTakenEventHandler;
            _endGame += endGameEventHandler;
            _cellGrid = CreateCellArray();
            _sets = CreateSets().ToArray();
            //Canvas = new Canvas();
        }

        private List<Cell[]> CreateSets()
        {
            var cells = new List<Cell[]>();
            cells.Add(_allCells.Where(c => c.I + c.J == 2).ToArray());
            cells.Add(_allCells.Where(c => c.I == c.J).ToArray());
            cells.AddRange(_allCells.GroupBy(c => c.I, k => k).Select(g => g.ToArray()));     
            cells.AddRange(_allCells.GroupBy(c => c.J, k => k).Select(g => g.ToArray()));
            return cells;
        }

        public Cell[] GameCells 
        {
            get
            {
                return _cellGrid;
            }
        }

        public void TakePlayerTurn(Guid playerId, Cell cell)
        {
            GameCells.Single(c => c.I == cell.I && c.J == cell.J).UpdateValue(_players[playerId]);
            if (GameOver())
            {
                _endGame.Invoke(this, new EndGameEventArgs() { Winner = playerId, WinningSet = GetWinningSet() });
                _currentPlayerId = Guid.Empty;
            }
            else 
                _currentPlayerId = _players.Keys.FirstOrDefault(p => p != playerId);
                //_currentPlayerId = _players.Keys.First(p => p != playerId);
            _turnTaken.Invoke(this, null);
        }

        internal void StartAtEndGame()
        {
            _currentPlayerId = _players.Keys.First();
            GameCells.Single(c => c.I == 1 && c.J == 1).UpdateValue(Marker.X);
            GameCells.Single(c => c.I == 2 && c.J == 1).UpdateValue(Marker.O);
            GameCells.Single(c => c.I == 0 && c.J == 2).UpdateValue(Marker.X);
            GameCells.Single(c => c.I == 2 && c.J == 0).UpdateValue(Marker.O);
            GameCells.Single(c => c.I == 2 && c.J == 2).UpdateValue(Marker.X);
            GameCells.Single(c => c.I == 1 && c.J == 2).UpdateValue(Marker.O);
        }

        public void StartRandomPlayer()
        {
            _currentPlayerId = _players.Keys.First();
        }

        public bool GameOver()
        {
            return _sets.Any(s => Math.Abs(s.Sum(c => (int)(c.Marker ?? 0))) == 3);
        }

        private Cell[] GetWinningSet()
        {
            return _sets.Where(s => Math.Abs(s.Sum(c => (int)(c.Marker ?? 0))) == 3).First();
        }

        public Cell[] CreateCellArray()
        {
            var cellArray = new Cell[9];
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    cellArray[count] = new Cell(i, j);
                    _allCells.Add(cellArray[count]);
                    count++;
                }
            }
            return cellArray;
        }

        public GameState GetCurrentState()
        {
            return new GameState(_currentPlayerId, _cellGrid);
        }

        public void AddPlayerToGame(Guid playerId)
        {
            var marker = !_players.Any() ? Marker.X : Marker.O;
            _players[playerId] = marker;
        }
    }   
}