using Realms;
using Store1.Enums;
using System;

namespace Store1
{
    public class SentOrderStatusEntry : RealmObject
    {
        public DateTimeOffset Date { get; set; }

        public string Status { get; set; }
        public string ReceiverName { get; set; }

    }
}