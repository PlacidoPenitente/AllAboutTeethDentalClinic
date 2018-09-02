using AllAboutTeethDCMS.Menu;
using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Drawing;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace AllAboutTeethDCMS
{
    public abstract class PageViewModel : ViewModelBase
    {
        #region Fields
        private MenuViewModel menuViewModel;
        private MainWindowViewModel mainWindowViewModel;
        private User activeUser;
        #endregion

        #region Properties
        public MenuViewModel MenuViewModel { get => menuViewModel; set => menuViewModel = value; }
        public MainWindowViewModel MainWindowViewModel { get => mainWindowViewModel; set => mainWindowViewModel = value; }
        public User ActiveUser { get => activeUser; set => activeUser = value; }
        #endregion

        #region Methods
        public MySqlConnection CreateConnection()
        {
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = "server=localhost; database='allaboutteeth_database'; user='root';";
            connection.Open();
            return connection;
        }

        public object LoadObject(object model, int key = 0)
        {
            string prefix = model.GetType().Name;
            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM allaboutteeth_"+prefix+"s" + " WHERE " + prefix + "_No=@no";
                    command.Parameters.AddWithValue("@no", key);
                    List<object> temp = new List<object>();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            foreach (PropertyInfo info in model.GetType().GetProperties())
                            {
                                if (info.PropertyType.ToString().Equals("System.String"))
                                {
                                    info.SetValue(model, reader.GetString(prefix + "_" + info.Name));
                                }
                                else if (info.PropertyType.ToString().Equals("System.Int32"))
                                {
                                    info.SetValue(model, reader.GetInt32(prefix + "_" + info.Name));
                                }
                                else if (info.PropertyType.ToString().Equals("System.DateTime"))
                                {
                                    info.SetValue(model, reader.GetDateTime(prefix + "_" + info.Name));
                                }
                                else if (info.PropertyType.ToString().Equals("System.Boolean"))
                                {
                                    info.SetValue(model, reader.GetBoolean(prefix + "_" + info.Name));
                                }
                                else if (info.PropertyType.ToString().Equals("System.Double"))
                                {
                                    info.SetValue(model, reader.GetDouble(prefix + "_" + info.Name));
                                }
                                else
                                {
                                    object infoTemp = Activator.CreateInstance(info.PropertyType);
                                    infoTemp.GetType().GetProperty("No").SetValue(infoTemp, reader.GetInt32(prefix + "_" + info.Name));
                                    info.SetValue(model, infoTemp);
                                    temp.Add(infoTemp);
                                }
                            }
                            reader.Close();
                            connection.Close();
                            foreach (object info in temp)
                            {
                                LoadObject(info, (int)info.GetType().GetProperty("No").GetValue(info));
                            }
                            return model;
                        }
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            return null;
        }

        public string convertToString(ImageSource image)
        {
            MemoryStream ms = new MemoryStream();
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image));
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
