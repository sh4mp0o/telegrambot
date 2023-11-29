using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace telegrambot
{
    interface ISerialization
    {
        void Serialization(List<Client> _clients);
        List<Client> Deserialization();
    }
    internal class JSONSerialization : ISerialization
    {

        public void Serialization(List<Client> _clients)
        {
            var clientsindification = from clients in _clients where (clients.Confirmation == true) select clients;

            var json = new DataContractJsonSerializer(typeof(List<Client>), new DataContractJsonSerializerSettings());
            using (FileStream fstream = new FileStream("Clients.json", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                json.WriteObject(fstream, clientsindification);
            }
        }
        public List<Client> Deserialization()
        {
            List<Client> _client;
            var json = new DataContractJsonSerializer(typeof(List<Client>));
            try
            {
                using (FileStream fstream = System.IO.File.OpenRead("Clients.json"))
                {
                    _client = (List<Client>)json.ReadObject(fstream);
                }
            }
            catch (Exception) { _client = new(); }
            return _client;
        }
    }
    internal class SerializationOfClient
    {
        public ISerialization ContextStrategy { get; set; }

        public List<Client> Clients { get; set; }
        public SerializationOfClient(ISerialization _strategy)
        {
            ContextStrategy = _strategy;
        }

        public void Serialization(List<Client> _clients)
        {
            ContextStrategy.Serialization(_clients);
        }
        public List<Client> Deserialization()
        {
            return ContextStrategy.Deserialization();
        }
    }
}
