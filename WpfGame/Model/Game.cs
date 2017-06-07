using System;
using System.Reflection;
using System.Threading.Tasks;
using IBot;
using TicTacToe;

namespace WpfGame.Model
{
    public class Game
    {
        private readonly IPlayer[] _players = new IPlayer[2];
        private int _currentPlayerIndex;

        private Game(Field field, IPlayer firstPlayer, IPlayer secondPlayer)
        {
            Field = field;
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
        }

        public Field Field { get; }
        public IPlayer FirstPlayer { get; }
        public IPlayer SecondPlayer { get; }
        public IPlayer CurrentStepPlayer => _players[_currentPlayerIndex];
        public bool IsEnded { get; private set; } = true;

        public static Game Instance { get; private set; }

        public event EventHandler<TurnedEventArgs> Turned;

        private void ConnectPlayerToTurnedEvent(IPlayer player)
        {
            player.Turned += OnTurned;
        }

        public Task Begin()
        {
            IsEnded = false;

            ConnectPlayerToTurnedEvent(FirstPlayer);
            ConnectPlayerToTurnedEvent(SecondPlayer);

            _players[0] = FirstPlayer;
            _players[1] = SecondPlayer;
            _currentPlayerIndex = -1;

            return NextPlayerStep();
        }

        public Task NextPlayerStep()
        {
            _currentPlayerIndex++;
            _currentPlayerIndex %= 2;

            return CurrentStepPlayer.SetCanTurn();
        }

        private void OnTurned(object sender, TurnedEventArgs eventArgs)
        {
            if (Field.IsEnd())
            {
                IsEnded = true;
            }

            Turned?.Invoke(sender, eventArgs);
        }

        public static void CreateWithBothHumans()
        {
            Field field = new Field();
            IPlayer firstPlayer = new HumanPlayer(field, CellState.Tick);
            IPlayer secondPlayer = new HumanPlayer(field, CellState.Tack);

            Instance = new Game(field, firstPlayer, secondPlayer);
        }

        public static void CreateWithFirstBot(Assembly botFrom, IBotSelector botSelector)
        {
            Field field = new Field();

            AbstractBot firstBot = botSelector.GetBotFromAssembly(botFrom, field, CellState.Tick);

            IPlayer firstPlayer = new BotPlayer(firstBot);
            IPlayer secondPlayer = new HumanPlayer(field, CellState.Tack);

            Instance = new Game(field, firstPlayer, secondPlayer);
        }

        public static void CreateWithSecondBot(Assembly botFrom, IBotSelector botSelector)
        {
            Field field = new Field();

            AbstractBot secondBot = botSelector.GetBotFromAssembly(botFrom, field, CellState.Tack);

            IPlayer firstPlayer = new HumanPlayer(field, CellState.Tick);
            IPlayer secondPlayer = new BotPlayer(secondBot);

            Instance = new Game(field, firstPlayer, secondPlayer);
        }

        public static void CreateWithBothBot(Assembly firstBotFrom, Assembly secondBotFrom, IBotSelector botSelector)
        {
            Field field = new Field();

            AbstractBot firstBot = botSelector.GetBotFromAssembly(firstBotFrom, field, CellState.Tick);
            AbstractBot secondBot = botSelector.GetBotFromAssembly(secondBotFrom, field, CellState.Tack);

            IPlayer firstPlayer = new BotPlayer(firstBot);
            IPlayer secondPlayer = new BotPlayer(secondBot);

            Instance = new Game(field, firstPlayer, secondPlayer);
        }
    }
}