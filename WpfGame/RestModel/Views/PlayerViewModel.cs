using TicTacToe;

namespace WpfGame.RestModel.Views
{
    public class PlayerViewModel
    {        
        public string Id { get; set; }
        public string Name { get; set; }
        public CellState State { get; set; }
    }
}
