using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store1
{
    public class OrderEntry : RealmObject//e chiar definitia modelului
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public IList<SentOrderStatusEntry> SentOrderStatuses { get; }//legatura cu un alt obiect realm din BD

        public SentOrderStatusEntry LastStatus { get; set; }

        public DateTimeOffset Date => LastStatus.Date;
    }
}
