using System;
using System.Collections.Generic;
using System.Linq;
using IBot;
using TicTacToe;

namespace MaxBot
{
    public class CommonBot : AbstractBot
    {
        private static readonly Random _random = new Random();

        public CommonBot(Field field, CellState state)
            : base(field, state)
        {
        }

        public override Cell Step()
        {
            List<Point> candidates = FilterCellsStageThree();
            if (candidates.Count == 0)
            {
                return new Cell(0, 0, State);
            }
            int index = _random.Next(candidates.Count);
            return new Cell(candidates[index], State);
        }

        private List<Point> FilterCellsStageOne()
        {
            return FilterCellsCore(GetNearestPoints(), EstimateForStageOne);
        }

        private List<Point> FilterCellsStageTwo()
        {
            return FilterCellsCore(FilterCellsStageOne(), EstimateForStageTwo);
        }

        private List<Point> FilterCellsStageThree()
        {
            return FilterCellsCore(FilterCellsStageTwo(), EstimateForStageThree);
        }

        private List<Point> FilterCellsCore(IEnumerable<Point> source, EstimateFunction estimator)
        {
            List<Point> result = new List<Point>();
            IComparable bestEstimate = null;

            foreach (Point point in source)
            {
                if (Field[point].State == CellState.Empty)
                {
                    IComparable estimate = estimator(point);

                    int compareResult = estimate.CompareTo(bestEstimate);

                    if (!(compareResult < 0))
                    {
                        if (compareResult > 0)
                        {
                            result.Clear();
                            bestEstimate = estimate;
                        }
                        result.Add(point);
                    }
                }
            }

            return result;
        }

        internal IComparable EstimateForStageOne(Point point)
        {
            int selfScore = CalcScore(point, State);
            int opponentScore = CalcScore(point, OpponentState);

            if (selfScore >= Field.VictorySequencySize)
            {
                selfScore = int.MaxValue;
            }

            return Math.Max(selfScore, opponentScore);
        }

        internal IComparable EstimateForStageTwo(Point point)
        {
            int selfCount = 0;
            int opponentCount = 0;
            const int DISTANCE = 2;

            for (int x = point.X - DISTANCE; x <= point.X + DISTANCE; x++)
            {
                for (int y = point.Y - DISTANCE; y <= point.Y + DISTANCE; y++)
                {
                    if (Field[x, y].State == State)
                    {
                        selfCount++;
                    }
                    else if (Field[x, y].State == OpponentState)
                    {
                        opponentCount++;
                    }
                }
            }

            return 2 * selfCount + opponentCount;
        }

        internal IComparable EstimateForStageThree(Point point)
        {
            return -Math.Sqrt(point.X * point.X + point.Y * point.Y);
        }

        private List<Point> GetNearestPoints()
        {
            List<Point> marked = Field.GetMarkedCells().Select(cell => new Point(cell.X, cell.Y)).ToList();
            HashSet<Point> result = new HashSet<Point>();
            foreach (Point point in marked)
            {
                for (int dx = -Field.VictorySequencySize + 1; dx < Field.VictorySequencySize - 1; dx++)
                {
                    for (int dy = -Field.VictorySequencySize + 1; dy < Field.VictorySequencySize - 1; dy++)
                    {
                        result.Add(new Point(point.X + dx, point.Y + dy));
                    }
                }
            }
            return result.ToList();
        }

        protected int CalcScore(Point location, CellState state)
        {
            int[] counts =
            {
                CalcScoreInDirection(location, -1, 0, state) + CalcScoreInDirection(location, 1, 0, state),
                CalcScoreInDirection(location, 0, -1, state) + CalcScoreInDirection(location, 0, 1, state),
                CalcScoreInDirection(location, -1, -1, state) + CalcScoreInDirection(location, 1, 1, state),
                CalcScoreInDirection(location, -1, 1, state) + CalcScoreInDirection(location, 1, -1, state)
            };

            return counts.Max() + 1;
        }

        private int CalcScoreInDirection(Point start, int dx, int dy, CellState state)
        {
            int result = 0;

            for (int i = 1; i < Field.VictorySequencySize; i++)
            {
                Point current = new Point(start.X + i * dx, start.Y + i * dy);
                if (Field[current].State != state)
                {
                    break;
                }
                result++;
            }

            return result;
        }

        private delegate IComparable EstimateFunction(Point point);
    }
}