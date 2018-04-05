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
        #region Fields
        private MenuViewModel menuViewModel;
        private MySqlConnection connection;
        private MainWindowViewModel mainWindowViewModel;
        private User activeUser;
        #endregion

        #region Properties
        public MenuViewModel MenuViewModel { get => menuViewModel; set => menuViewModel = value; }
        public MySqlConnection Connection { get => connection; set => connection = value; }
        public MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }
        public User ActiveUser { get => activeUser; set => activeUser = value; }
        #endregion

        #region Methods
        public void CreateConnection()
        {
            try
            {
                if (Connection == null)
                {
                    Connection = new MySqlConnection();
                    Connection.ConnectionString = "server=localhost; database='allaboutteeth_database'; user='docnanz'; password='docnanz';";
                    Connection.Open();
                }
                if (Connection.State != System.Data.ConnectionState.Open)
                {
                    Connection.Open();
                }
            }
            catch(Exception ex)
            {

            }
        }
        #endregion
    }
}
