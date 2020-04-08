using Store1.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Store1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreEntriesPage : ContentPage
    {
        public StoreEntriesPage()
        {
            InitializeComponent();
            BindingContext = new StoreEntriesViewModel { Navigation = Navigation };
        }

        void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            (BindingContext as StoreEntriesViewModel).EditEntry((OrderEntry)e.Item);
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }
    }
}