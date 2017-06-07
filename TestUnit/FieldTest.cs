using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacToe;

namespace TestUnit
{
    /// <summary>
    ///     Тестирование поля игры
    /// </summary>
    [TestClass]
    public class FieldTest
    {
        /// <summary>
        ///     Конструктор по умолчанию создает пустое поле
        /// </summary>
        [TestMethod]
        public void TestFieldConstructor()
        {
            Field field = new Field();
            for (long x = Int32.MinValue; x < Int32.MaxValue; x += Math.Abs(x) / 2 + 1)
            {
                for (long y = Int32.MinValue; y < Int32.MaxValue; y += Math.Abs(y) / 2 + 1)
                {
                    Assert.AreEqual(CellState.Empty, field[(int)x, (int)y].State);
                    Assert.AreEqual(x, field[(int)x, (int)y].X);
                    Assert.AreEqual(y, field[(int)x, (int)y].Y);
                }
            }
        }

        /// <summary>
        ///     Успешный ход крестиком
        /// </summary>
        [TestMethod]
        public void TestTurnTickSuccess()
        {
            Field field = new Field();
            field.Turn(CellState.Tick, 0, 0);
            Assert.AreEqual(CellState.Tick, field[0, 0].State);
            Assert.AreEqual(CellState.Empty, field[0, 1].State);
            Assert.AreEqual(CellState.Empty, field[1, 0].State);
            Assert.AreEqual(CellState.Empty, field[1, 1].State);
            Assert.AreEqual(CellState.Empty, field[0, -1].State);
            Assert.AreEqual(CellState.Empty, field[-1, 0].State);
            Assert.AreEqual(CellState.Empty, field[-1, -1].State);
            Assert.AreEqual(CellState.Empty, field[-1, 1].State);
            Assert.AreEqual(CellState.Empty, field[1, -1].State);
            Assert.AreEqual(CellState.Empty, field[Int32.MaxValue, Int32.MaxValue].State);
        }

        /// <summary>
        ///     Успешный ход ноликом
        /// </summary>
        [TestMethod]
        public void TestTurnTackSuccess()
        {
            Field field = new Field();
            field.Turn(CellState.Tack, 0, 0);
            Assert.AreEqual(CellState.Tack, field[0, 0].State);
            Assert.AreEqual(CellState.Empty, field[0, 1].State);
            Assert.AreEqual(CellState.Empty, field[1, 0].State);
            Assert.AreEqual(CellState.Empty, field[1, 1].State);
            Assert.AreEqual(CellState.Empty, field[0, -1].State);
            Assert.AreEqual(CellState.Empty, field[-1, 0].State);
            Assert.AreEqual(CellState.Empty, field[-1, -1].State);
            Assert.AreEqual(CellState.Empty, field[-1, 1].State);
            Assert.AreEqual(CellState.Empty, field[1, -1].State);
            Assert.AreEqual(CellState.Empty, field[Int32.MaxValue, Int32.MaxValue].State);
        }

        /// <summary>
        ///     Успешный ход на максимальные координаты поля
        /// </summary>
        [TestMethod]
        public void TestTurnWithMaxXYSuccess()
        {
            Field field = new Field();
            field.Turn(CellState.Tick, Int32.MaxValue, Int32.MaxValue);
            Assert.AreEqual(CellState.Tick, field[Int32.MaxValue, Int32.MaxValue].State);
        }

        /// <summary>
        ///     Успешный ход на минимальные координаты поля
        /// </summary>
        [TestMethod]
        public void TestTurnWithMinXYSuccess()
        {
            Field field = new Field();
            field.Turn(CellState.Tick, Int32.MinValue, Int32.MinValue);
            Assert.AreEqual(CellState.Tick, field[Int32.MinValue, Int32.MinValue].State);
        }

        /// <summary>
        ///     Попытка пойти в занятую ячейку должна приводить к исключению
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException), "Нельзя пойти пустым местом", AllowDerivedTypes = true)]
        public void TestTurnEmptyCellFail()
        {
            Field field = new Field();
            field.Turn(CellState.Tick, 0, 1);
            field.Turn(CellState.Empty, 0, 0);
        }

        /// <summary>
        ///     Попытка пойти в занятую ячейку должна приводить к исключению
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException), "Нельзя пойти в занятую ячейку", AllowDerivedTypes = true)]
        public void TestTurnToOccupatedCellFail()
        {
            Field field = new Field();
            field.Turn(CellState.Tick, 0, 0);
            field.Turn(CellState.Tack, 0, 0);
        }

        /// <summary>
        ///     Попытка пойти в занятую ячейку должна приводить к исключению
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException), "Нельзя пойти в занятую ячейку", AllowDerivedTypes = true)]
        public void TestAnywayTurnToOccupatedCellFail()
        {
            Field field = new Field();
            field.Turn(CellState.Tick, 0, 0);
            field.Turn(CellState.Tack, 0, 1);
            Assert.AreEqual(CellState.Tick, field[0, 0].State);
            Assert.AreEqual(CellState.Tack, field[0, 1].State);
            field.Turn(CellState.Tick, 0, 0);
        }

        /// <summary>
        ///     Попытка пойти в два раза подряд одной фигурой приводит к исключению
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException), "Нельзя пойти дважды.", AllowDerivedTypes = true)]
        public void TestTurnTwiceWithTickFail()
        {
            Field field = new Field();
            field.Turn(CellState.Tick, 0, 0);
            field.Turn(CellState.Tick, 0, 1);
        }

        /// <summary>
        ///     Попытка пойти в два раза подряд одной фигурой приводит к исключению
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException), "Нельзя пойти дважды.", AllowDerivedTypes = true)]
        public void TestTurnTwiceWithTackFail()
        {
            Field field = new Field();
            field.Turn(CellState.Tack, 0, 0);
            field.Turn(CellState.Tack, 0, 1);
        }

        /// <summary>
        ///     Проверка на окончание игры
        /// </summary>
        [TestMethod]
        [Timeout(200)]
        public void TestGameEnd1()
        {
            Field field = new Field();

            PlayTurns(field, CellState.Tick,
                new[]
                {
                    new[] { 0, 0 }, new[] { 0, 1 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { -1, 0 }, new[] { -1, 1 },
                    new[] { 2, 0 }, new[] { 2, 1 }, new[] { -2, 0 }
                });
            Assert.IsTrue(field.IsEnd());
        }

        /// <summary>
        ///     Проверка на окончание игры
        /// </summary>
        [TestMethod]
        [Timeout(200)]
        public void TestGameEnd2()
        {
            Field field = new Field();

            PlayTurns(field, CellState.Tack,
                new[]
                {
                    new[] { 0, 0 }, new[] { 1, 0 }, new[] { 0, 1 }, new[] { 1, 1 }, new[] { 0, -1 }, new[] { -1, 1 },
                    new[] { 0, 2 }, new[] { 2, 1 }, new[] { 0, -2 }
                });
            Assert.IsTrue(field.IsEnd());
        }

        /// <summary>
        ///     Проверка на окончание игры
        /// </summary>
        [TestMethod]
        [Timeout(200)]
        public void TestGameEnd3()
        {
            Field field = new Field();

            PlayTurns(field, CellState.Tack,
                new[]
                {
                    new[] { 0, 0 }, new[] { -10, 10 }, new[] { 1, 1 }, new[] { 11, 1 }, new[] { -1, -1 },
                    new[] { -11, 1 }, new[] { 2, 2 }, new[] { 2, 1 }, new[] { -2, -2 }
                });
            Assert.IsTrue(field.IsEnd());
        }

        /// <summary>
        ///     Проверка на окончание игры
        /// </summary>
        [TestMethod]
        [Timeout(200)]
        public void TestGameEnd4()
        {
            Field field = new Field();
            PlayTurns(field, CellState.Tick,
                new[]
                {
                    new[] { 0, 0 }, new[] { -1, 11 }, new[] { -1, 1 }, new[] { 14, 1 }, new[] { 1, -1 },
                    new[] { -41, 1 }, new[] { -2, 2 }, new[] { -3, 3 }, new[] { 2, -2 }
                });
            Assert.IsTrue(field.IsEnd());
        }

        /// <summary>
        ///     Получить ячейки в которых стоит крестик или нолик
        /// </summary>
        [TestMethod]
        [Timeout(200)]
        public void TestGetMarkedCells()
        {
            Field field = new Field();
            PlayTurns(field, CellState.Tick,
                new[]
                {
                    new[] { 0, 0 }, new[] { -1, 11 }, new[] { -1, 1 }, new[] { 14, 1 }, new[] { 1, -1 },
                    new[] { -41, 1 }, new[] { -2, 2 }, new[] { -3, 3 }, new[] { 2, -2 }
                });

            IList<Cell> cells = field.GetMarkedCells();
            Assert.AreEqual(9, cells.Count);
            foreach (Cell cell in cells)
            {
                Assert.AreNotEqual(CellState.Empty, cell.State);
            }
        }

        private void PlayTurns(Field field, CellState whoBegins, int[][] positions)
        {
            CellState currentPlayer = whoBegins;
            foreach (int[] position in positions)
            {
                Assert.IsFalse(field.IsEnd());
                field.Turn(currentPlayer, position[0], position[1]);
                currentPlayer = currentPlayer == CellState.Tick ? CellState.Tack : CellState.Tick;
            }
        }
    }
}