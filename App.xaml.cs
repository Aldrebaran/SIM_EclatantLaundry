using System.Configuration;
using System.Data;
using System.Windows;
using EclatantLaundry.Models;
using EclatantLaundry.Services;


namespace EclatantLaundry
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AuthService AuthService { get; private set; } = null!;

        public App()
        {
            var context = new AppDbContext();
            AuthService = new AuthService(context);
        }
    }

}
