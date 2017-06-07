using System;
using TicTacToe;

namespace IBot
{
    public abstract class AbstractBot
    {
        private CellState _myCellState;

        protected AbstractBot(Field field, CellState state)
        {
            Field = field;
            State = state;
        }

        public Field Field { get; set; }

        public CellState OpponentState { get; private set; }

        public CellState State
        {
            get => _myCellState;
            set
            {
                if (value == CellState.Empty)
                {
                    throw new ArgumentException("Бот не может ходить пустым местом");
                }

                _myCellState = value;
                OpponentState = _myCellState == CellState.Tick ? CellState.Tack : CellState.Tick;
            }
        }

        public abstract Cell Step();
    }
}