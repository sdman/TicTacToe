using System.Windows;

namespace WpfGame
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            LogoutService.Logout(null, null);
            base.OnExit(e);
        }
    }
}