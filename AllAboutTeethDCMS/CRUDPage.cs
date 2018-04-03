using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace AllAboutTeethDCMS
{
    public abstract class CRUDPage<T> : PageViewModel where T : new()
    {
        #region Create
        protected void SaveToDatabase(T model, string tableName)
        {
            CreateConnection();
            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "INSERT INTO " + tableName + " VALUES (";
            foreach (PropertyInfo info in model.GetType().GetProperties())
            {
                if (info.Name.Equals("No"))
                {
                    command.CommandText += "NULL,";
                }
                else if (info.Name.Equals("DateAdded") || info.Name.Equals("DateModified"))
                {
                    command.CommandText += "NOW(),";
                }
                else
                {
                    command.CommandText += "@" + info.Name + ",";
                }
            }
            command.CommandText = command.CommandText.Remove(command.CommandText.Length - 1) + ");";

            foreach (PropertyInfo info in model.GetType().GetProperties())
            {
                if (info.Name.Equals("AddedBy"))
                {
                    command.Parameters.AddWithValue("@" + info.Name, ActiveUser.No);
                }
                else if (info.Name.Equals("Dentist") ||
                    info.Name.Equals("Appointment") ||
                    info.Name.Equals("Tooth") ||
                    info.Name.Equals("Supplier") ||
                    info.Name.Equals("Patient") ||
                    info.Name.Equals("Owner") ||
                    info.Name.Equals("Treatment"))
                {
                    command.Parameters.AddWithValue("@" + info.Name,
                        info.GetValue(model).GetType().GetProperty("No").GetValue(info.GetValue(model)));
                }
                else if (!info.Name.Equals("No") &&
                    !info.Name.Equals("DateAdded") &&
                    !info.Name.Equals("DateModified"))
                {
                    command.Parameters.AddWithValue("@" + info.Name, info.GetValue(model));
                }
            }

            command.ExecuteNonQuery();
            Connection.Close();
        }

        #endregion

        #region Read
        protected List<T> LoadFromDatabase(string tableName, string filter)
        {
            Progress = 0;
            string prefix = new T().GetType().Name;
            List<T> list = new List<T>();
            List<int> primaryKeys = new List<int>();

            CreateConnection();
            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "SELECT * FROM " + tableName;
            if (!filter.Trim().Equals(""))
            {
                command.CommandText += " WHERE";
                foreach (PropertyInfo info in new T().GetType().GetProperties())
                {
                    command.CommandText += " " + prefix + "_" + info.Name + " LIKE @" + info.Name + " OR ";
                }
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 3);
                foreach (PropertyInfo info in new T().GetType().GetProperties())
                {
                    command.Parameters.AddWithValue("@" + info.Name, "%" + filter + "%");
                }
            }
            else
            {
                beforeLoad(command);
            }
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                primaryKeys.Add(reader.GetInt32(prefix + "_No"));
            }
            reader.Close();
            Connection.Close();

            foreach (int primaryKey in primaryKeys)
            {
                list.Add((T)LoadItem(new T(), tableName, primaryKey));
                Progress = ((double)list.Count / primaryKeys.Count) * 100;
            }
            try
            {
            }
            catch(Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(ex.Message);
                Console.BackgroundColor = ConsoleColor.Black;
            }
            return list;
        }

        protected object LoadItem(object model, string tableName, int key = 0)
        {
            string prefix = model.GetType().Name;

            CreateConnection();
            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "SELECT * FROM " + tableName + " WHERE " + prefix + "_No=@no";

            command.Parameters.AddWithValue("@no", key);
            List<object> temp = new List<object>();

            MySqlDataReader reader = command.ExecuteReader();
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
                        info.SetValue(model, reader.GetDecimal(prefix + "_" + info.Name));
                    }
                    else
                    {
                        var infoTemp = Activator.CreateInstance(info.PropertyType);
                        infoTemp.GetType().GetProperty("No").SetValue(infoTemp, reader.GetInt32(prefix + "_" + info.Name));
                        info.SetValue(model, infoTemp);
                        temp.Add(infoTemp);
                    }
                }
                reader.Close();
                Connection.Close();

                foreach (object info in temp)
                {
                    if(!info.GetType().Name.Equals("Tooth"))
                    {
                        LoadItem(info, "allaboutteeth_" + info.GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), (int)info.GetType().GetProperty("No").GetValue(info));
                    }
                    else
                    {
                        LoadItem(info, "allaboutteeth_tooths", (int)info.GetType().GetProperty("No").GetValue(info));
                    }
                }

                return model;
            }

            reader.Close();
            Connection.Close();
            return null;
        }
        #endregion

        #region Update
        public void UpdateDatabase(T model, string tableName)
        {
            string prefix = model.GetType().Name;

            CreateConnection();
            MySqlCommand command = Connection.CreateCommand();

            command.CommandText = "UPDATE " + tableName + " SET ";
            int no = 0;
            foreach (PropertyInfo info in model.GetType().GetProperties())
            {
                if (info.Name.Equals("No"))
                {
                    no = (int)info.GetValue(model);
                }
                if ((!info.Name.Equals("No")) && (!info.Name.Equals("DateAdded")) && (!info.Name.Equals("DateModified")))
                {
                    command.CommandText += prefix + "_" + info.Name + "=@" + info.Name + ",";
                }
            }
            command.CommandText = command.CommandText.Remove(command.CommandText.Length - 1) + " WHERE " + prefix + "_No=@key";

            command.Parameters.AddWithValue("@key", no);
            foreach (PropertyInfo info in model.GetType().GetProperties())
            {
                if (info.Name.Equals("AddedBy"))
                {
                    command.Parameters.AddWithValue("@" + info.Name, ActiveUser.No);
                }
                else if (info.Name.Equals("Dentist") ||
                    info.Name.Equals("Appointment") ||
                    info.Name.Equals("Tooth") ||
                    info.Name.Equals("Supplier") ||
                    info.Name.Equals("Patient") ||
                    info.Name.Equals("Owner") ||
                    info.Name.Equals("Treatment"))
                {
                    command.Parameters.AddWithValue("@" + info.Name,
                        info.GetValue(model).GetType().GetProperty("No").GetValue(info.GetValue(model)));
                }
                else if (!info.Name.Equals("No") &&
                    !info.Name.Equals("DateAdded") &&
                    !info.Name.Equals("DateModified"))
                {
                    command.Parameters.AddWithValue("@" + info.Name, info.GetValue(model));
                }
            }

            command.ExecuteNonQuery();
            Connection.Close();
        }
        #endregion

        #region Delete
        protected void DeleteFromDatabase(T model, string tableName)
        {
            string prefix = model.GetType().Name;
            CreateConnection();
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "DELETE FROM " + tableName + " WHERE " + prefix + "_No=@key";
            command.Parameters.AddWithValue("@key", model.GetType().GetProperty("No").GetValue(model));
            command.ExecuteNonQuery();
            Connection.Close();
        }
        #endregion

        #region Create Thread
        private Thread addThread;

        protected void startSaveToDatabase(T model, string tableName)
        {
            SpawnThread(addThread, model, tableName, executeSaveToDatabase);
        }

        private void executeSaveToDatabase()
        {
            if (beforeCreate())
            {
                try
                {
                    SaveToDatabase(Model, TableName);
                    afterCreate(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    afterCreate(false);
                }
            }
        }
        #endregion

        #region Read Thread
        private Thread loadThread;

        protected void startLoadFromDatabase(string tableName, string filter)
        {
            SpawnThread(loadThread, default(T), tableName, executeLoadFromDatabase, filter);
        }

        private void executeLoadFromDatabase()
        {
            List<T> list = LoadFromDatabase(TableName, Filter);
            afterLoad(list);
        }
        #endregion

        #region Update Thread
        private Thread saveThread;

        protected void startUpdateToDatabase(T model, string tableName)
        {
            SpawnThread(saveThread, model, tableName, executeUpdateToDatabase);
        }

        private void executeUpdateToDatabase()
        {
            if (beforeUpdate())
            {
                try
                {
                    UpdateDatabase(Model, TableName);
                    afterUpdate(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    afterUpdate(false);
                }
            }
        }
        #endregion

        #region Delete Thread
        private Thread deleteThread;

        protected void startDeleteFromDatabase(T model, string tableName)
        {
            SpawnThread(deleteThread, model, tableName, executeDeleteFromDatabase);
        }

        private void executeDeleteFromDatabase()
        {
            if (beforeDelete())
            {
                try
                {
                    DeleteFromDatabase(Model, TableName);
                    afterDelete(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    afterDelete(false);
                }
            }

        }
        #endregion

        private void SpawnThread(Thread thread, T model, string tableName, ThreadStart parameterizedThreadStart, string filter = "")
        {
            if (thread == null || !thread.IsAlive)
            {
                TableName = tableName;
                Model = model;
                Filter = filter;
                thread = new Thread(parameterizedThreadStart);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public CRUDPage()
        {
            DialogBoxViewModel = new DialogBoxViewModel();
        }

        #region Abstract Methods
        protected abstract bool beforeCreate();
        protected abstract void afterCreate(bool isSuccessful);
        protected abstract void beforeLoad(MySqlCommand command);
        protected abstract void afterLoad(List<T> list);
        protected abstract bool beforeUpdate();
        protected abstract void afterUpdate(bool isSuccessful);
        protected abstract bool beforeDelete();
        protected abstract void afterDelete(bool isSuccessful);
        #endregion

        #region Fields
        private DialogBoxViewModel dialogBoxViewModel;
        private double progress = 0;
        private string tableName = "";
        private string filter = "";
        private string filterResult = "";
        private T model;
        #endregion

        #region Properties
        public DialogBoxViewModel DialogBoxViewModel { get => dialogBoxViewModel; set { dialogBoxViewModel = value; OnPropertyChanged(); } }
        public double Progress { get => progress; set { progress = value; OnPropertyChanged(); } }
        public string TableName { get => tableName; set => tableName = value; }
        public string Filter { get => filter; set => filter = value; }
        public T Model { get => model; set => model = value; }
        public string FilterResult { get => filterResult; set { filterResult = value; OnPropertyChanged(); } }
        #endregion

        protected bool Validate(string value)
        {
            if(String.IsNullOrEmpty(value.Trim()))
            {
                return false;
            }
            return true;
        }

        protected bool ValidateNumberOnly(string value)
        {
            foreach (char c in value.ToArray())
            {
                if (!Char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        protected bool ValidateLetterAndNumberOnly(string value)
        {
            foreach (char c in value.ToArray())
            {
                if (!Char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        protected string validate(string value)
        {
            if(String.IsNullOrEmpty(value.Trim()))
            {
                return "This field is required.";
            }
            return "";
        }

        protected string validateContact(string value)
        {
            if (String.IsNullOrEmpty(value.Trim()))
            {
                return "This field is required.";
            }

            foreach (char c in value.ToCharArray())
            {
                if(!Char.IsDigit(c))
                {
                    return "Illegal characters found.";
                }
            }
            return "";
        }

        protected string validatePassword(string value)
        {
            if (String.IsNullOrEmpty(value.Trim()))
            {
                return "This field is required.";
            }

            if (value.Trim().Length > 60)
            {
                return "Cannot be more than 60 characters.";
            }

            if (value.Trim().Length<5)
            {
                return "Must be atleast 5 characters.";
            }
            return "";
        }

        protected string ValidateUsername(string value, string original)
        {
            try
            {
                if (Connection == null)
                {
                    CreateConnection();
                }
                if (Connection.State != System.Data.ConnectionState.Open)
                {
                    Connection.Open();
                }
                MySqlCommand command = Connection.CreateCommand();
                command.CommandText = "SELECT * FROM allaboutteeth_users WHERE user_username=@user";
                command.Parameters.AddWithValue("@user", value);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if(reader.GetString("user_username").Equals(original))
                    {
                        reader.Close();
                        Connection.Close();
                        return "";
                    }
                    reader.Close();
                    Connection.Close();
                    return "Username is already taken.";
                }
                reader.Close();
                Connection.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Unable to check username.";
            }
            return "";
        }

        protected string validateUniqueName(string value, string original, string tableName)
        {
            if (String.IsNullOrEmpty(value.Trim()))
            {
                return "This field is required.";
            }
            if (!value.Trim().Equals(original))
            {
                if(Connection==null)
                {
                    CreateConnection();
                }
                if (Connection.State != System.Data.ConnectionState.Open)
                {
                    Connection.Open();
                }
                MySqlCommand command = Connection.CreateCommand();
                command.CommandText = "SELECT * FROM "+tableName+" WHERE "+ new T().GetType().Name +"_name=@name";
                command.Parameters.AddWithValue("@name", value);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                    Connection.Close();
                    return "Name is already taken.";
                }
                reader.Close();
                Connection.Close();
            }
            return "";
        }
    }
}
