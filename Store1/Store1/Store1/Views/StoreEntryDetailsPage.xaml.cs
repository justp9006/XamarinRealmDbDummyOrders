using Store1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Store1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreEntryDetailsPage : ContentPage
    {
        public StoreEntryDetailsPage()
        {
            InitializeComponent();
            BindingContext = new StoreEntriesViewModel { Navigation = Navigation };
        }
        public StoreEntryDetailsPage(StoreEntryDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            viewModel.Navigation = Navigation;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as StoreEntryDetailsViewModel)?.OnDisappearing();
            BindingContext = null;
        }
    }
}