using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TicTacToe;
using WpfGame.Annotations;
using WpfGame.Model;
using WpfGame.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using Point = TicTacToe.Point;

namespace WpfGame.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string FirstPlayerText = "Ходит игрок (крестики).";
        private const string SecondPlayerText = "Ходит игрок (нолики).";
        private const string FirstPlayerWinText = "Выиграл первый игрок (крестики).";
        private const string SecondPlayerWinText = "Выиграл второй игрок (нолики).";

        private const int DefaultRowsCount = 15;
        private const int DefaultColumnsCount = 15;

        private bool ended;

        private readonly ICommand _cellLeftClickCommand;
        private readonly ICommand _cellRightClickCommand;
        private readonly ObservableCollection<CellViewModel> _cells;

        //private readonly Game _gameInstance;
        private string _stepInfoText;
        private IEnumerable<Point> _winPoints;

        private CellState currentState;
        private CellState myState;

        private NewWaitWindow waitWindow;

        private MainWindow mainWindow;        

        private Dictionary<Point, CellState> field = new Dictionary<Point, CellState>();

        public MainViewModel(MainWindow _mainWindow, NewWaitWindow _waitWindow, CellState _myState)
        {
            ended = false;
            myState = _myState;
            currentState = CellState.Tick;
            _cellLeftClickCommand = new RelayCommand(OnCellLeftClicked);
            _cellRightClickCommand = new RelayCommand(OnCellRightClicked, CanCellRightClick);
            RestartCommand = new RelayCommand(OnRestart);
            waitWindow = _waitWindow;
            mainWindow = _mainWindow;

            //_gameInstance = Game.Instance;
            //_gameInstance.Turned += OnTurned;
            //_gameInstance.Win += OnWin;
                        

            _cells = CreateCellViews();
            UpdateCellViews(0, 0);

            //_gameInstance.Begin();
            StepInfoText = FirstPlayerText;

            WebSocket.connection.On("GameStarted", () =>
            {                                
                mainWindow.Dispatcher.Invoke(() => { mainWindow.Show(); });
                waitWindow.Dispatcher.Invoke(() => { waitWindow.Close(); });                
            });

            WebSocket.connection.On("GameEnded", (string cause) =>
            {
                if (cause == "disconnected")
                {
                    RestartNeeded?.Invoke(this, null);
                    MessageBox.Show("Connection losed.", "disconnected");
                }                
            });

            WebSocket.connection.On<CellState, int, int>("StepMade", (CellState cellState, int x, int y) =>
            {
                if (!ended)
                {
                    CellViewModel vm = _cells.FirstOrDefault(c => c.X == x && c.Y == y);

                    // Если vm == null, то ход происходит вне видимого поля и его не надо рисовать.
                    if (vm != null)
                    {
                        UpdateCell(vm, cellState);
                    }

                    if (cellState == CellState.Tack)
                    {
                        currentState = CellState.Tick;
                        StepInfoText = SecondPlayerText;
                    }
                    else if (cellState == CellState.Tick)
                    {
                        currentState = CellState.Tack;
                        StepInfoText = FirstPlayerText;
                    }                    
                    field.Add(new Point(x, y), cellState);                    
                }                
            });

            WebSocket.connection.On("Vin", (IEnumerable<Point> vinPoints) =>
            {
                ended = true;

                foreach (Point point in vinPoints)
                {
                    CellViewModel cellVm = _cells.FirstOrDefault(c => c.X == point.X && c.Y == point.Y);

                    if (cellVm != null)
                    {
                        cellVm.CellColor = Colors.Red;
                    }                    
                }
            });

            Task.Run(async () => { await WebSocket.connection.StartAsync();  });
        }

        public int Rows => DefaultRowsCount;
        public int Columns => DefaultColumnsCount;
        public IEnumerable<CellViewModel> Cells => _cells;
        public ICommand RestartCommand { get; }

        public string StepInfoText
        {
            get => _stepInfoText;
            set
            {
                if (value == _stepInfoText)
                {
                    return;
                }

                _stepInfoText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler RestartNeeded;

        //private async void OnTurned(object sender, TurnedEventArgs turnedEventArgs)
        //{
        //    CellViewModel vm = _cells.FirstOrDefault(c => c.X == turnedEventArgs.X && c.Y == turnedEventArgs.Y);

        //    // Если vm == null, то ход происходит вне видимого поля и его не надо рисовать.
        //    if (vm != null)
        //    {
        //        UpdateCell(vm, turnedEventArgs.CellState);
        //    }

        //    await _gameInstance.NextPlayerStep();

        //    if (!_gameInstance.IsEnded)
        //    {
        //        StepInfoText = _gameInstance.CurrentStepPlayer == _gameInstance.FirstPlayer ? FirstPlayerText
        //            : SecondPlayerText;
        //    }
        //}

        //private void OnWin(object sender, WinEventArgs winEventArgs)
        //{
        //    StepInfoText = winEventArgs.Winner == _gameInstance.FirstPlayer ? FirstPlayerWinText : SecondPlayerWinText;

        //    _winPoints = _gameInstance.Field.GetWinningPoints();

        //    if (_winPoints == null)
        //    {
        //        return;
        //    }

        //    foreach (Point point in _winPoints)
        //    {
        //        CellViewModel cellVm = _cells.FirstOrDefault(c => c.X == point.X && c.Y == point.Y);

        //        if (cellVm != null)
        //        {
        //            cellVm.CellColor = Colors.Red;
        //        }
        //    }
        //}

        private void OnCellLeftClicked(object args)
        {
            CellViewModel cellViewModel = (CellViewModel)args;
            UpdateCellViews(cellViewModel.X, cellViewModel.Y);
        }

        private async void OnCellRightClicked(object args)
        {
            CellViewModel cellViewModel = (CellViewModel)args;
            await WebSocket.connection.InvokeAsync ("OnStep", cellViewModel.X, cellViewModel.Y); ;
        }

        private bool CanCellRightClick(object args)
        {
            return !ended && myState == currentState;
        }

        private void OnRestart(object args)
        {
            RestartNeeded?.Invoke(this, null);
        }

        private ObservableCollection<CellViewModel> CreateCellViews()
        {
            ObservableCollection<CellViewModel> result = new ObservableCollection<CellViewModel>();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result.Add(new CellViewModel(0, 0, _cellLeftClickCommand, _cellRightClickCommand));
                }
            }

            return result;
        }

        private void UpdateCellViews(int x, int y)
        {
            int beginRow = x - Rows / 2;
            int endRow = x + Rows / 2;

            int beginColumn = y - Columns / 2;
            int endColumn = y + Columns / 2;

            int cellViewIndex = 0;

            for (int currentX = beginRow; currentX <= endRow; currentX++)
            {
                for (int currentY = beginColumn; currentY <= endColumn; currentY++)
                {
                    Point p = new Point(currentX, currentY);
                    CellState cellAtPoint;

                    if (field.ContainsKey(p))
                    {
                        cellAtPoint = field[p];
                    }
                    else
                    {
                        cellAtPoint = CellState.Empty;
                    }
                    

                    CellViewModel cellViewModel = _cells[cellViewIndex];
                    cellViewModel.X = currentX;
                    cellViewModel.Y = currentY;

                    UpdateCell(cellViewModel, cellAtPoint);

                    cellViewIndex++;
                }
            }
        }

        private void UpdateCell(CellViewModel cellViewModel, CellState state)
        {
            switch (state)
            {
                case CellState.Tick:
                    cellViewModel.Sign = "X";
                    break;

                case CellState.Tack:
                    cellViewModel.Sign = "O";
                    break;

                case CellState.Empty:
                    cellViewModel.Sign = String.Empty;
                    break;

                default: throw new ArgumentOutOfRangeException();
            }

            if (_winPoints != null && _winPoints.Any(p => p.X == cellViewModel.X && p.Y == cellViewModel.Y))
            {
                cellViewModel.CellColor = Colors.Red;
            }
            else
            {
                cellViewModel.CellColor = Colors.White;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}