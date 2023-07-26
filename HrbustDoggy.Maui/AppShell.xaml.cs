using HrbustDoggy.Maui.Views;

namespace HrbustDoggy.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("login", typeof(LoginPage));
        }
    }
}