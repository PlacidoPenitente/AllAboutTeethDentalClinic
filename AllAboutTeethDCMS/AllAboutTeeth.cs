using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AllAboutTeethDCMS
{
    public class AllAboutTeeth
    {
        #region Fields
        private string title = "Strong N' White Dental Clinic Co.";
        private UserControl activePage;
        private User activeUser;
        private Database database;
        #endregion

        #region Properties
        public string Title { get => title; set => title = value; }
        public UserControl ActivePage { get => activePage; set => activePage = value; }
        public User ActiveUser { get => activeUser; set => activeUser = value; }
        public Database Database { get => database; set => database = value; }
        #endregion

        public AllAboutTeeth()
        {
            Database = new Database();
        }
    }
}
