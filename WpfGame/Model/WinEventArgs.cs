using System;

namespace WpfGame.Model
{
    public class WinEventArgs : EventArgs
    {
        public IPlayer Winner { get; set; }
    }
}