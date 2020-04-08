using Store1.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Store1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new StoreEntriesPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
