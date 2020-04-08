using Realms;
using Store1.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Store1.ViewModels
{
    public class StoreEntriesViewModel
    {
        // TODO: add UI for changing that.
        private const string SenderName = "Me";
        private string[] Receivers = { "John", "Mary", "George", "Elizabeth", "Vasile", "Marghioala", "Tanase" };

        private Realm _realm;//utilizat pt interactiunea cu BD
        public OrderEntry Entry { get; private set; }
        public IEnumerable<OrderEntry> Entries { get; private set; }

        public ICommand AddEntryCommand { get; private set; }

        public ICommand DeleteEntryCommand { get; private set; }

        public INavigation Navigation { get; set; }

        public StoreEntriesViewModel()
        {
            _realm = Realm.GetInstance();//ia instanta pentru thread-ul asta
            //NU e bine sa pasezi obiecte realm de la un thread la altul
            //daca deschid un task run pun acolo alt Realm.GetInstance()

            Entry = new OrderEntry();
            Entries = _realm.All<OrderEntry>();

            AddEntryCommand = new Command(AddEntry);
            DeleteEntryCommand = new Command<OrderEntry>(DeleteEntry);
        }

        private void AddEntry()
        {
            var transaction = _realm.BeginWrite();
            var entry = _realm.Add(new OrderEntry
            {
                Title = Entry.Title,
                Description = Entry.Description,

            });
            for (int i = 0; i < 7; i++)
            {
                var sentOrderStatus = new SentOrderStatusEntry
                {
                    Date = DateTimeOffset.Now,
                    ReceiverName = Receivers[i]
                };
                entry.SentOrderStatuses.Add(sentOrderStatus);
            }
            // Entry = entry;

            var page = new StoreEntryDetailsPage(new StoreEntryDetailsViewModel(entry, transaction));
            //var page = new StoreEntryDetailsPage(new StoreEntryDetailsViewModel());
            Navigation.PushAsync(page);
        }

        internal void EditEntry(OrderEntry entry)
        {
            var transaction = _realm.BeginWrite();

            var page = new StoreEntryDetailsPage(new StoreEntryDetailsViewModel(entry, transaction));

            Navigation.PushAsync(page);
        }

        private void DeleteEntry(OrderEntry entry)
        {
            _realm.Write(() => _realm.Remove(entry));
        }
    }
}
