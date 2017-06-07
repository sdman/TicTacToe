using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    public class Field
    {
        private int VICTORY_SEQUENCE_SIZE = 5;
        //private List<List<Cell>> _cells;
        private Dictionary<Point, Cell> _cells;
        private Cell _lastTurn;
        //private Tuple<int, int> _center;

        public Field()
        {
            const int START_CAPACITY = 50;
            const int MIDDLE = START_CAPACITY >> 1;
            _center = new Tuple<int, int>(MIDDLE, MIDDLE);

            _cells = new List<List<Cell>>(START_CAPACITY);
            for (int i = 0; i < START_CAPACITY; i++)
            {
                _cells.Add((new Cell[START_CAPACITY]).ToList());
                for(int j = 0; j < START_CAPACITY; j++)
                {
                    _cells[i][j] = new Cell(i - MIDDLE, j - MIDDLE);
                }
            }
        }

        public Cell this[int x, int y] {
            get
            {
                int translatedX = TranslateX(x);
                int translatedY = TranslateY(y);
                if (translatedX < 0 || translatedX > _cells.Count || 
                    translatedY < 0 || translatedY > _cells[0].Count)
                {
                    return new Cell(x, y);
                }
                return _cells[translatedX][translatedY];
            }
        }

        private int TranslateX(int x)
        {
            return _center.Item1 + x;
        }

        private int TranslateY(int y)
        {
            return _center.Item2 + y;
        }

        public void Turn(CellState state, int x, int y)
        {
            CheckEmptyState(state);
            CheckDoubleTurn(state);
            CheckOccupiedCell(x, y);

            x = TranslateX(x);
            y = TranslateY(y);
            CheckFieldBorders(x, y);

            _cells[x][y].State = state;
            _lastTurn = _cells[x][y];
        }

        private void CheckEmptyState(CellState state)
        {
            if (state == CellState.Empty)
            {
                throw new ApplicationException("Нельзя пойти пустым местом");
            }
        }

        private void CheckDoubleTurn(CellState state)
        {
            if (_lastTurn != null && _lastTurn.State == state)
            {
                throw new ApplicationException("Нельзя пойти дважды.");
            }
        }

        private void CheckOccupiedCell(int x, int y)
        {
            if (this[x, y].State != CellState.Empty)
            {
                throw new ApplicationException("Нельзя пойти в занятую ячейку.");
            }
        }

        private void CheckFieldBorders(int translatedX, int translatedY)
        {
            const int GAP = 5;
            if (translatedX < 0)
            {
                LeftHorizontalExpansion(Math.Abs(translatedX) + GAP);
            }
            else if (translatedX > _cells[0].Count)
            {
                RightHorizontalExpansion(translatedX - _cells[0].Count + GAP);
            }

            if (translatedY < 0)
            {
                TopVerticalExpansion(Math.Abs(translatedY) + GAP);
            }
            else if (translatedY > _cells.Count)
            {
                BottomVerticalExpansion(translatedY - _cells.Count + GAP);
            }
        }

        private void LeftHorizontalExpansion(int expansion)
        {
            int THE_MOST_LEFT = _cells[0][0].X;
            for (int i = 0; i < _cells[0].Count; i++)
            {
                _cells[i] = (new Cell[expansion]).Concat(_cells[i]).ToList();
                for (int j = 0; j < expansion; j++)
                {
                    _cells[i][j] = new Cell(THE_MOST_LEFT - expansion + j, _cells[i][0].Y);
                }
            }

            _center = new Tuple<int, int>(_center.Item1 + expansion, _center.Item2);
        }

        private void RightHorizontalExpansion(int expansion)
        {
            int THE_MOST_RIGHT = _cells[0][0].X + _cells[0].Count;
            for (int i = 0; i < _cells.Count; i++)
            {
                _cells[i] = _cells[i].Concat(new Cell[expansion]).ToList();
                for (int j = 0; j < expansion; j++)
                {
                    _cells[i][j] = new Cell(THE_MOST_RIGHT + j, _cells[i][0].Y);
                }
            }
        }

        private void TopVerticalExpansion(int expansion)
        {
            var newRows = MakeRows(expansion);
            _cells = newRows.Concat(_cells).ToList();

            for (int i = 0; i < expansion; i++)
            {
                for(int j = 0; j < _cells[0].Count; j++)
                {
                    _cells[i][j] = new Cell(_cells[expansion + 1][j].X, )
                }
            }
            _center = new Tuple<int, int>(_center.Item1, _center.Item2 + expansion);
        }

        private void BottomVerticalExpansion(int expansion)
        {
            var newRows = MakeRows(expansion);
            _cells = _cells.Concat(newRows).ToList();
        }

        private List<List<Cell>> MakeRows(int size)
        {
            List<List<Cell>> result = new List<List<Cell>>(size);
            for (int i = 0; i < size; i++)
            {
                result.Add((new Cell[_cells[0].Count]).ToList());
            }
            return result;
        }

        public bool IsEnd()
        {
            return _lastTurn != null ? CalcMaxScore() >= VICTORY_SEQUENCE_SIZE : false;
        }

        private int CalcMaxScore()
        {
            int[] scores = new int[] {
                CountScoreInDirection(-1, 0) + CountScoreInDirection(1, 0),
                CountScoreInDirection(0, -1) + CountScoreInDirection(0, 1),
                CountScoreInDirection(-1, -1) + CountScoreInDirection(1, 1),
                CountScoreInDirection(-1, 1) + CountScoreInDirection(1, -1)
            };

            int result = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                result = Math.Max(result, scores[i]);
            }
            return result;
        }

        private int CountScoreInDirection(int dx, int dy)
        {
            int result = 0;
            CellState opponent = GetOpponent(_lastTurn.State);
            for (int i = 1; i < VICTORY_SEQUENCE_SIZE; i++)
            {
                if (this[_lastTurn.X + i * dx, _lastTurn.Y + i * dy].State == opponent)
                {
                    break;
                }       
                result++;
            }
            return result;
        }

        private CellState GetOpponent(CellState state)
        {
            if(state == CellState.Empty)
            {
                return state;
            } else
            {
                return state == CellState.Tick ? CellState.Tack : CellState.Tick;
            }
        }

        public List<Cell> GetMarkedCells()
        {
            List<Cell> result = new List<Cell>();
            foreach(var row in _cells)
            {
                foreach(var cell in row)
                {
                    if(cell.State != CellState.Empty)
                    {
                        result.Add(cell);
                    }
                }
            }
            return result;
        }
    }
}
