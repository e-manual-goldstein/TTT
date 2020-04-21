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

namespace TTT.Core
{
    public class Game
    {
        Cell[,] _cellGrid;
        List<Cell> _allCells = new List<Cell>();
        Cell[][] _sets;
        float _screenWidth;
        float _screenHeight;
        Func<float> _widthFunc;
        Func<float> _heightFunc;
        Dictionary<Guid, Marker> _players = new Dictionary<Guid, Marker>();
        Guid _currentPlayerId;

        public Game(float screenWidth, float screenHeight) : this()
        {
            
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            Canvas = new Canvas();
            Canvas.Width = screenWidth;
            Canvas.Height = screenHeight;
            
        }

        public Game(Func<float> widthFunc, Func<float> heightFunc) : this(widthFunc(), heightFunc())
        {
            _widthFunc = widthFunc;
            _heightFunc = heightFunc;
        }

        public Game()
        {
            _cellGrid = CreateCellArray();
            _sets = CreateSets().ToArray();
            Canvas = new Canvas();
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

        public Canvas Canvas { get; set; }

        public Cell[,] GameCells 
        {
            get
            {
                return _cellGrid;
            }
        }


        public void TakePlayerTurn(Guid playerId, Cell cell)
        {
            GameCells[cell.I, cell.J].UpdateValue(_players[playerId]);
            if (GameOver())
            {
                EndGame.Invoke(this, new EndGameEventArgs() { Winner = playerId, WinningSet = GetWinningSet() });
                _currentPlayerId = Guid.Empty;
            }
            else 
                _currentPlayerId = _players.Keys.FirstOrDefault(p => p != playerId);
                //_currentPlayerId = _players.Keys.First(p => p != playerId);
            TurnTaken.Invoke(this, null);
        }

        internal void StartAtEndGame()
        {
            _currentPlayerId = _players.Keys.First();
            _cellGrid[1, 1].UpdateValue(Marker.X);
            _cellGrid[2, 1].UpdateValue(Marker.O);
            _cellGrid[0, 2].UpdateValue(Marker.X);
            _cellGrid[2, 0].UpdateValue(Marker.O);
            _cellGrid[2, 2].UpdateValue(Marker.X);
            _cellGrid[1, 2].UpdateValue(Marker.O);
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

        public EventHandler TurnTaken { get; set; }

        public EventHandler<EndGameEventArgs> EndGame { get; set; }

        //public Cell[] CreateCells()
        //{
        //    var cells = new List<Cell>();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            var cell = new Cell(i, j);
        //            cells.Add(cell);
        //        }
        //    }
        //    return cells.ToArray();
        //}

        public Cell[,] CreateCellArray()
        {
            var cellArray = new Cell[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    cellArray[i,j] = new Cell(i, j);
                    _allCells.Add(cellArray[i, j]);
                }
            }
            return cellArray;
        }


        public void DrawCells(float width, float height)
        {
            Canvas.Width = width;
            Canvas.Height = height;

            var baseX = (width - (3 * Constants.CellSizeHost)) / 2;
            var baseY = (height - (3 * Constants.CellSizeHost)) / 2;

            foreach (var cell in _cellGrid)
            {
                var cellCanvas = DrawCell(cell, baseX, baseY);
                Canvas.Children.Add(cellCanvas);
            }
        }

        public Canvas DrawCell(Cell cell, float baseX, float baseY)
        {
            var cellCanvas = new Canvas();
            cellCanvas.Width = Constants.CellSizeHost;
            cellCanvas.Height = Constants.CellSizeHost;
            Canvas.SetLeft(cellCanvas, baseX + (cell.I * Constants.CellSizeHost));
            Canvas.SetTop(cellCanvas, baseY + (cell.J * Constants.CellSizeHost));
            cellCanvas.Background = new SolidColorBrush(Colors.Gray);
            cellCanvas.Children.Add(DrawTextBox(cell, Constants.CellSizeHost));
            return cellCanvas;
        }

        public Label DrawTextBox(Cell cell, int size)
        {
            var label = new Label();
            label.Height = size - 2;
            label.Width = size - 2;
            label.Background = new SolidColorBrush(Colors.White);
            label.FontSize = 40;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            Canvas.SetBottom(label, 1);
            Canvas.SetLeft(label, 1);
            cell.SetUpdateValueAction((newValue) => UpdateCellValue(label, newValue, () => cell.Active));
            return label;
        }

        public void UpdateCellValue(Label label, Marker newValue, Func<bool> active)
        {
            App.Current.Dispatcher.Invoke(() => 
            {
                label.Content = newValue.ToString();
                if (active())
                    label.Background = new SolidColorBrush(Colors.Tomato);
            });
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