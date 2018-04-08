using AllAboutTeethDCMS.Menu;
using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing;

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
        //public MySqlConnection Connection { get => connection; set => connection = value; }
        public MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }
        public User ActiveUser { get => activeUser; set => activeUser = value; }
        #endregion

        #region Methods
        public MySqlConnection CreateConnection()
        {
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = "server=localhost; database='allaboutteeth_database'; user='docnanz'; password='docnanz';";
            connection.Open();
            return connection;
            //if (Connection == null)
            //{
                
            //}
            //else
            //{
            //    if(Connection.State!=System.Data.ConnectionState.Open)
            //    {
            //        Connection.Open();
            //    }
            //}
        }

        public string convertToString(ImageSource image)
        {
            MemoryStream ms = new MemoryStream();
            var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(image as System.Windows.Media.Imaging.BitmapSource));
            encoder.Save(ms);
            ms.Flush();
            Image Image = Image.FromStream(ms);

            string base64String = "";
            MemoryStream m = new MemoryStream();
            Image.Save(m, Image.RawFormat);
            byte[] imageBytes = m.ToArray();
            base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        #endregion
    }
}
