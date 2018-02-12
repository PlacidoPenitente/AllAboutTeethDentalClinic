using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS
{
    public class Database
    {
        private string address = "";
        private string portNumber = "";
        private string databaseName = "";
        private string username = "";
        private string password = "";

        public string Address { get => address; set => address = value; }
        public string PortNumber { get => portNumber; set => portNumber = value; }
        public string DatabaseName { get => databaseName; set => databaseName = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
    }
}
