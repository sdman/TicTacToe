using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBot;
using TicTacToe;

namespace TicTacToeBot
{
    public class Bot : AbstractBot
    {
        struct GamePattern
        {
            public string pattern;
            public int weight;

            public GamePattern(string _pattern, int _weight)
            {
                pattern = _pattern;
                weight = _weight;
            }
        }

        // преобразует значения CellState в "0","1","2".
        private Dictionary<CellState, String> _XOmap = new Dictionary<CellState, string>();

        // Паттерны атаки бота.
        GamePattern[] _attackPatterns =
        {
            new GamePattern("11111", 100000),
            new GamePattern("011110", 7000),
            new GamePattern("011112", 3000),
            new GamePattern("211110", 3000),
            new GamePattern("0101110", 3000),
            new GamePattern("0111010", 3000),
            new GamePattern("0011100", 2000),
            new GamePattern("1010101", 2000),
            new GamePattern("001112", 1000),
            new GamePattern("211100", 1000),
            new GamePattern("00110100", 1000),
            new GamePattern("00101100", 1000),
            new GamePattern("00011000", 200)
        };
        
        // Паттерны обороны бота.
        GamePattern[] _defPatterns =
        {
            new GamePattern("22222", 100000),
            new GamePattern("022220", 7000),
            new GamePattern("022221", 4000),
            new GamePattern("122220", 4000),
            new GamePattern("0202220", 4000),
            new GamePattern("0222020", 4000),
            new GamePattern("0022200", 2000),
            new GamePattern("2020202", 2000),
            new GamePattern("002221", 1000),
            new GamePattern("122200", 1000),
            new GamePattern("00220200", 1000),
            new GamePattern("00202200", 1000),
            new GamePattern("00022000", 200),
        };

        string _botXO;
        string _enemyXO;

        Random _random = new Random();

        public Bot(Field field, CellState cellState) 
            : base(field, cellState)
        {
            _botXO = "1";
            _enemyXO = "2";

            _XOmap.Add(CellState.Empty, "0");
            _XOmap.Add(State, _botXO);
            _XOmap.Add(OpponentState, _enemyXO);
        }

        override public Cell Step()
        {
            List<Point> markedPoints = Field.GetMarkedCells().Select(cell => new Point(cell.X, cell.Y)).ToList();

            if (markedPoints.Count != 0)
            {
                IEnumerable<Point> potentialPoints = GetPotentialPoints(markedPoints);                

                float maxSum = -1;
                float curSum = 0;
                List<Point> candidates = new List<Point>();

                foreach (var point in potentialPoints)
                {
                    curSum = PatternSum(point.X, point.Y, _attackPatterns, _botXO) * 1.1f + 
                            PatternSum(point.X, point.Y, _defPatterns, _enemyXO);

                    if (curSum >= maxSum)
                    {
                        if (curSum > maxSum)
                        {
                            candidates.Clear();
                            maxSum = curSum;
                        }
                        candidates.Add(point);
                    } 
                }
                int index = _random.Next(candidates.Count);
                return new Cell(candidates[index], State);
            } else
            {
                return new Cell(0, 0, State);
            }
        }

        private IEnumerable<Point> GetPotentialPoints(List<Point> markedPoints)
        {
            for (int i = 0; i < markedPoints.Count; i++)
            {
                for (int row = -2; row <= 2; row++)
                {
                    for (int colomn = -2; colomn <= 2; colomn++)
                    {
                        if ((row != 0) || (colomn != 0))
                        {
                            if (Field[markedPoints[i].X + row, markedPoints[i].Y + colomn].State == CellState.Empty)
                            {
                                yield return new Point(markedPoints[i].X + row, markedPoints[i].Y + colomn);
                            }
                        }
                    }
                }
            }
        }

        private int PatternSum(int X, int Y, GamePattern[] botPatterns, string XO)
        {
            int sum = 0;

            string[] lines = new string[4];

            //Вертикальная линия.
            for (int index = -4; index < 5; index++)
            {
                if (0 == index)
                    lines[0] += XO;
                else
                    lines[0] += _XOmap[Field[X + index, Y].State];
            }

            //Диагональная линия с уклоном вправо.
            for (int index = -4; index < 5; index++)
            {
                if (0 == index)
                    lines[1] += XO;
                else
                    lines[1] += _XOmap[Field[X - index, Y + index].State];
            }

            //Горизонтальная линия.
            for (int index = -4; index < 5; index++)
            {
                if (0 == index)
                    lines[2] += XO;
                else
                    lines[2] += _XOmap[Field[X, Y + index].State];
            }

            //Диагональная линия с уклоном влево.
            for (int index = -4; index < 5; index++)
            {
                if (0 == index)
                    lines[3] += XO;
                else
                    lines[3] += _XOmap[Field[X + index, Y + index].State];
            }

            for (int i = 0; i < 4; i++)
            {
                for (int p = 0; p < botPatterns.Length; p++)
                {
                    if (lines[i].Contains(botPatterns[p].pattern))
                    {
                        sum += botPatterns[p].weight;
                        break;
                    }
                }
            }
            return sum;
        }
    }
}
