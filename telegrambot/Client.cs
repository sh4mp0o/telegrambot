using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegrambot
{
    [Serializable]
    internal class Client:IComparable<Client>
    {
        private DateTime _dateTime = DateTime.Today;
        private string _time;
        private string _name;
        private long _id;
        private Survey _survey;
        private bool _confirmation = false;
        private string _username;
        private string _phone;

        public Client() { }

        public long Id
        {
            get => _id;
            init => _id = value;
        }
        public string Time
        {
            get => _time;
            set => _time = value;
        }
        public DateTime DateTime
        {
            get => _dateTime; 
            set => _dateTime = value;
        }
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        public string Phone
        {
            get => _phone;
            set => _phone = value;
        }
        public bool Confirmation { set => _confirmation = value; get => _confirmation; }
        public Survey Survey { get => _survey;}

        public int CompareTo(Client? other)
        {
            if (this.DateTime > other.DateTime)
                return 1;
            if (this.DateTime < other.DateTime)
                return -1;
            return 0;
        }
    }
}
