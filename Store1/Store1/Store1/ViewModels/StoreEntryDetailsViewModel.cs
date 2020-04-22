using System;
using System.Collections.Generic;
using System.Text;
using Realms;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading;
using Store1.Enums;
using System.Linq;
using Store1.Base;

namespace Store1.ViewModels
{
    public class StoreEntryDetailsViewModel : ObservableProperty
    {
        private Transaction _transaction;

        private Realm _realm => Realm.GetInstance();//utilizat pt interactiunea cu BD
        private bool selectOrderStatusForReceiverVisibility = false;
        private SentOrderStatusEntry selectedSentOrderStatus;
        public OrderEntry Entry { get; private set; }

        public IList<OrderStatus> Statuses { get; private set; }
        public OrderStatus SelectedStatus { get; set; }
        public IEnumerable<OrderEntry> Entries { get; private set; }
        public SentOrderStatusEntry SelectedSentOrderStatus
        {
            get
            {
                return selectedSentOrderStatus;
            }
            set
            {
                selectedSentOrderStatus = value;
                if (value != null)
                    SelectOrderStatusForReceiverVisibility = true;
                OnPropertyChanged("SelectedSentOrderStatus");
            }
        }

        public bool SelectOrderStatusForReceiverVisibility
        {
            get
            {
                return selectOrderStatusForReceiverVisibility;
            }
            set
            {
                selectOrderStatusForReceiverVisibility = value;
                OnPropertyChanged("SelectOrderStatusForReceiverVisibility");
            }
        }
        internal INavigation Navigation { get; set; }

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteOrderCommand { get; private set; }
        public ICommand SaveLocalCommand { get; private set; }

        public StoreEntryDetailsViewModel(OrderEntry entry, Transaction transaction)
        {
            //Lista cu statusuri hardcodate din care sa aleg cand schimb statusul unuia
            Statuses = new List<OrderStatus>();
            Statuses.Add(OrderStatus.Local);
            Statuses.Add(OrderStatus.Cloud);
            Statuses.Add(OrderStatus.Delivered);
            Statuses.Add(OrderStatus.Read);
            //Entries = _realm.All<OrderEntry>();//aici iau absolut toate comenzile din BD - in pagina asta nu imi mai trebuie. am tot modificat dar las codul momentan
            Entry = entry;//Entry e comanda selectata in lucru in pagina asta de detalii
            _transaction = transaction;//tranzactia a fost deschisa in pagina principala si data ca parametru aici. Cand o inchid sa salveaza efectiv in BD
            ////entry.Title = $"Titlu {DateTime.Now.ToString()}";
            ////entry.Description = "BodyText";
            //_transaction.Commit();
            SaveCommand = new Command(Save);
            SaveLocalCommand = new Command(SaveLocal);
            DeleteOrderCommand = new Command(DeleteOrder);
        }
        public StoreEntryDetailsViewModel()
        {
            Entries = _realm.All<OrderEntry>();
            SaveCommand = new Command(Save);
            SaveLocalCommand = new Command(SaveLocal);
            DeleteOrderCommand = new Command(DeleteOrder);
        }
        private void Save()
        {
            //for (int i = 0; i < Entry.SentOrderStatuses.Count; i++)
            //    Entry.SentOrderStatuses[i].Status = SelectedStatus.ToString();
            var selectedSentOrderStatus = Entry.SentOrderStatuses.FirstOrDefault((sentStatus) =>//intoarce-mi prima aparitie
            {
                if (SelectedSentOrderStatus == null)
                    return false;
                //daca am selectat un status de  comanda trimisa catre un destinatar
                return sentStatus.ReceiverName.Equals(SelectedSentOrderStatus.ReceiverName);//intoarce-mi primul Status de comanda gasit din lista de statusuri a comenzii aferente paginii curente (Entry)
            });
            if (selectedSentOrderStatus != null)//daca am gasit un status de comanda existent
                selectedSentOrderStatus.Status = SelectedStatus.ToString();//setez valoarea status a statusul de comanda cu valoarea statusului selectat din lista cu valori standard
            else if (Entry.Title != null && Entry.Title != string.Empty)
            {
                Application.Current.MainPage.DisplayAlert("Info", "No order receiver selected! No order status for has been modified", "Ok");
            }
            //Entry.SentOrderStatuses.Status = SelectedStatus.ToString();
            if (Entry.Title == null || Entry.Title == string.Empty)
            {
                Application.Current.MainPage.DisplayAlert("Error", "Every order must have a title! Changes have been discarded!", "Ok");
                _transaction.Dispose();//nu salvez nimic in BD daca nu am titlu
            }
            else
                _transaction.Commit();//termin tranzactia ca sa poata sa scrie in baza de date
            Navigation.PopAsync(true);//ies din pagina de detalii a comenzii in pagina generala unde pot vedea si comanda proaspat modificata
        }
        private void SaveLocal()
        {
            throw new NotImplementedException();
            //var transaction = _realm.BeginWrite();

            //Entry = _realm.Add(new OrderEntry
            //{
            //    Title = $"Titlu {DateTime.Now.ToString()}",
            //    Description = "BodyText",
            //    SentOrderStatuses = new SentOrderStatusEntry
            //    {
            //        Date = DateTimeOffset.Now,
            //        ReceiverName = "John"
            //    }
            //});
            //transaction.Commit();
            //// Navigation.PopAsync(true);
        }

        private void DeleteOrder()
        {
            Application.Current.MainPage.DisplayAlert("Info", "Delete order!", "Ok");

        }
        internal void OnDisappearing()
        {
            _transaction.Dispose();//renunt la tranzactie. nu se intampla nimic la nivelul BD
        }
    }
}
