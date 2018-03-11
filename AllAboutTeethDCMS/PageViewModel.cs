using AllAboutTeethDCMS.Menu;
using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS
{
    public abstract class PageViewModel : ViewModelBase
    {
        private MenuViewModel menuViewModel;
        private User activeUser;
        private MySqlConnection connection;

        public void createConnection()
        {
            connection = new MySqlConnection();
            connection.ConnectionString = "server=localhost; database='allaboutteeth_database'; user='docnanz'; password='docnanz';";
            connection.Open();
        }
        
        public MenuViewModel MenuViewModel { get => menuViewModel; set => menuViewModel = value; }
        public User ActiveUser { get => activeUser; set { activeUser = value; OnPropertyChanged(); } }
        public MySqlConnection Connection { get => connection; set => connection = value; }
    }
}
