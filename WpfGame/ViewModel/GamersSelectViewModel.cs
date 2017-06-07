using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfGame.Annotations;
using WpfGame.Model;

namespace WpfGame.ViewModel
{
    public class GamersSelectViewModel : INotifyPropertyChanged
    {
        private readonly IBotSelector _botSelector;
        private ICommand _gamersSelectCommand;

        private Visibility _isFirstAssembliesVisible = Visibility.Collapsed;
        private bool _isFirstHuman = true;

        private Visibility _isSecondAssembliesVisible = Visibility.Collapsed;
        private bool _isSecondHuman = true;

        private IEnumerable<Assembly> _loadedAssemblies;

        public GamersSelectViewModel(IBotSelector botSelector)
        {
            _botSelector = botSelector;
        }

        /// <summary>
        ///     Проверяет, первый игрок - человек или бот.
        /// </summary>
        public bool IsFirstHuman
        {
            get => _isFirstHuman;
            set
            {
                if (value == _isFirstHuman)
                {
                    return;
                }

                _isFirstHuman = value;
                IsFirstAssembliesVisible = _isFirstHuman ? Visibility.Collapsed : Visibility.Visible;

                OnPropertyChanged();
            }
        }

        public Visibility IsFirstAssembliesVisible
        {
            get => _isFirstAssembliesVisible;
            private set
            {
                if (_isFirstAssembliesVisible == value)
                {
                    return;
                }

                _isFirstAssembliesVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Проверяет, второй игрок - человек или бот.
        /// </summary>
        public bool IsSecondHuman
        {
            get => _isSecondHuman;
            set
            {
                if (value == _isSecondHuman)
                {
                    return;
                }

                _isSecondHuman = value;
                IsSecondAssembliesVisible = _isSecondHuman ? Visibility.Collapsed : Visibility.Visible;
                OnPropertyChanged();
            }
        }

        public Visibility IsSecondAssembliesVisible
        {
            get => _isSecondAssembliesVisible;
            private set
            {
                if (_isSecondAssembliesVisible == value)
                {
                    return;
                }

                _isSecondAssembliesVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand GamersSelectCommand => _gamersSelectCommand ??
                                               (_gamersSelectCommand =
                                                   new RelayCommand(OnPlayersSelected, CanSelectPlayers));

        public IEnumerable<Assembly> LoadedAssemblies => _loadedAssemblies ??
                                                         (_loadedAssemblies = _botSelector.GetBotAssemblies());

        public Assembly FirstBotAssembly { get; set; }
        public Assembly SecondBotAssembly { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler PlayersSelected;

        private void OnPlayersSelected(object arg)
        {
            if (IsFirstHuman && IsSecondHuman)
            {
                Game.CreateWithBothHumans();
            }

            else if (IsFirstHuman && !IsSecondHuman)
            {
                Game.CreateWithSecondBot(SecondBotAssembly, _botSelector);
            }

            else if (!IsFirstHuman && IsSecondHuman)
            {
                Game.CreateWithFirstBot(FirstBotAssembly, _botSelector);
            }

            else if (!IsFirstHuman && !IsSecondHuman)
            {
                Game.CreateWithBothBot(FirstBotAssembly, SecondBotAssembly, _botSelector);
            }

            PlayersSelected?.Invoke(this, null);
        }

        private bool CanSelectPlayers(object arg)
        {
            bool isFirstCorrect;
            bool isSecondCorrect;

            if (IsFirstHuman)
            {
                isFirstCorrect = true;
            }
            else
            {
                isFirstCorrect = FirstBotAssembly != null;
            }

            if (IsSecondHuman)
            {
                isSecondCorrect = true;
            }
            else
            {
                isSecondCorrect = SecondBotAssembly != null;
            }

            return isFirstCorrect && isSecondCorrect;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}