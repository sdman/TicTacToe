using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using TicTacToe;
using WpfGame.Annotations;
using WpfGame.Model;

namespace WpfGame.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string FirstPlayerText = "Ходит первый игрок (крестики).";
        private const string SecondPlayerText = "Ходит второй игрок (нолики).";
        private const string FirstPlayerWinText = "Выиграл первый игрок (крестики).";
        private const string SecondPlayerWinText = "Выиграл второй игрок (нолики).";

        private const int DefaultRowsCount = 15;
        private const int DefaultColumnsCount = 15;

        private readonly ICommand _cellLeftClickCommand;
        private readonly ICommand _cellRightClickCommand;
        private readonly ObservableCollection<CellViewModel> _cells;

        private readonly Game _gameInstance;
        private string _stepInfoText;
        private IEnumerable<Point> _winPoints;

        public MainViewModel()
        {
            _cellLeftClickCommand = new RelayCommand(OnCellLeftClicked);
            _cellRightClickCommand = new RelayCommand(OnCellRightClicked, CanCellRightClick);
            RestartCommand = new RelayCommand(OnRestart);

            _gameInstance = Game.Instance;
            _gameInstance.Turned += OnTurned;

            _cells = CreateCellViews();
            UpdateCellViews(0, 0);

            _gameInstance.Begin();
            StepInfoText = FirstPlayerText;
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

        private void OnTurned(object sender, TurnedEventArgs turnedEventArgs)
        {
            CellViewModel vm = _cells.FirstOrDefault(c => c.X == turnedEventArgs.X && c.Y == turnedEventArgs.Y);

            // Если vm == null, то ход происходит вне видимого поля и его не надо рисовать.
            if (vm != null)
            {
                UpdateCell(vm, turnedEventArgs.CellState);
            }

            if (!_gameInstance.IsEnded)
            {
                _gameInstance.NextPlayerStep();

                _winPoints = _gameInstance.Field.GetWinningPoints();

                StepInfoText = _gameInstance.CurrentStepPlayer == _gameInstance.FirstPlayer ? FirstPlayerText
                    : SecondPlayerText;
            }
            else
            {
                _winPoints = _gameInstance.Field.GetWinningPoints();

                foreach (Point point in _winPoints)
                {
                    CellViewModel cellVm = _cells.FirstOrDefault(c => c.X == point.X && c.Y == point.Y);

                    if (cellVm != null)
                    {
                        cellVm.CellColor = Colors.Red;
                    }
                }

                StepInfoText = _gameInstance.CurrentStepPlayer == _gameInstance.FirstPlayer ? FirstPlayerWinText
                    : SecondPlayerWinText;
            }
        }

        private void OnCellLeftClicked(object args)
        {
            CellViewModel cellViewModel = (CellViewModel)args;
            UpdateCellViews(cellViewModel.X, cellViewModel.Y);
        }

        private async void OnCellRightClicked(object args)
        {
            CellViewModel cellViewModel = (CellViewModel)args;
            await _gameInstance.CurrentStepPlayer.ForceTurn(cellViewModel.X, cellViewModel.Y);
        }

        private bool CanCellRightClick(object args)
        {
            return !_gameInstance.IsEnded && _gameInstance.CurrentStepPlayer is HumanPlayer;
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
                    Cell cellAtPoint = _gameInstance.Field[currentX, currentY];

                    CellViewModel cellViewModel = _cells[cellViewIndex];
                    cellViewModel.X = cellAtPoint.X;
                    cellViewModel.Y = cellAtPoint.Y;

                    UpdateCell(cellViewModel, cellAtPoint.State);

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