using Realms;
using Store1.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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
                LastStatus = new SentOrderStatusEntry
                {
                    Date = DateTimeOffset.Now,
                    ReceiverName = "Hardcoded test ReceiverName",
                    Status = "Hardcoded test Status"
                }
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
            //varianta veche, buna daca am relatie 1:1
            //_realm.Write(() => _realm.Remove(entry));

            //            Do the delete

            //Find an object we want to do a psuedo-cascading delete on -I'm directly using the LINQ First to get the object.

            //var delId = 1;
            //            var delP = realm.All<Product>().First(p => p.Id == delId);
            //            if (delP == null)
            //                return;
            //            Important fix to your sample - use ToList

            //realm.Write(() => {
            //    foreach (var r in delP.Reports.ToList())
            //        realm.Remove(r);
            //    realm.Remove(delP);  // lastly remove the parent 
            //});
            //Your approach in B was nearly correct but it ignores the fact that foreach (var report in currentProduct.Reports) is iterating a live list. Because the Reports container is updated each time you remove something, it will exit the loop before removing all the children.


            //varianta noua

            //var entryToBeDeleted = _realm.All<OrderEntry>().First(e => e.Title == entry.Title);//am pus titlul pe post de cheie primara doar ca sa vad ca merge
            //if (entryToBeDeleted == null)
            //    return;
            //_realm.Write(() =>
            //{
            //    foreach(var status in entryToBeDeleted.SentOrderStatuses.ToList())
            //    {
            //        _realm.Remove(status);
            //    }
            //    _realm.Remove(entryToBeDeleted);
            //    Entries = _realm.All<OrderEntry>();
            //    _realm.Refresh();
            //});

            //modifica valoarea titlului si in realm - se vede live in view
            //var entryToBeModified = _realm.All<OrderEntry>().First(e => e.Title == entry.Title);//am pus titlul pe post de cheie primara doar ca sa vad ca merge
            //if (entryToBeModified == null)
            //    return;
            //_realm.Write(() =>
            //{
            //    entryToBeModified.Title = "modificat";
            //    entryToBeModified.Description = "Description";

            //});

            var entries = _realm.All<OrderEntry>().Where<OrderEntry>(e => !e.Title.Equals(entry.Title));
            if (entries == null)
                return;
            Entries = entries.ToList();


            _realm.Write(() =>
            {
                _realm.RemoveAll<OrderEntry>();
                foreach (var e in Entries)
                {
                    _realm.Add<OrderEntry>(e,true);
                }
            });
        }
    }
}
