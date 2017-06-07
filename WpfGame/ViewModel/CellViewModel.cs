using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfGame.ViewModel
{
    public class CellViewModel : INotifyPropertyChanged
    {
        private bool _canSelect = true;
        private Color _cellColor = Colors.White;
        private string _sign;
        private int _x;
        private int _y;

        public CellViewModel(int x, int y, ICommand cellLeftClickCommand, ICommand cellRightClickCommand)
        {
            X = x;
            Y = y;

            CellLeftClickCommand = cellLeftClickCommand;
            CellRightClickCommand = cellRightClickCommand;
        }

        public int X
        {
            get => _x;
            set
            {
                if (value == _x)
                {
                    return;
                }

                _x = value;
                OnPropertyChanged();
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                if (value == _y)
                {
                    return;
                }

                _y = value;
                OnPropertyChanged();
            }
        }

        public string Sign
        {
            get => _sign;
            set
            {
                _sign = value;
                OnPropertyChanged();
            }
        }

        public Color CellColor
        {
            get => _cellColor;
            set
            {
                _cellColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand CellLeftClickCommand { get; }
        public ICommand CellRightClickCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}