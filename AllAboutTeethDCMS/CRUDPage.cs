using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Suppliers;
using AllAboutTeethDCMS.Treatments;
using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AllAboutTeethDCMS
{
    public abstract class CRUDPage<T> : PageViewModel where T : new()
    {
        private Thread loadThread;
        private Thread addThread;
        private Thread saveThread;
        private Thread deleteThread;

        protected void saveToDatabase(T model, string tableName)
        {
            if (Connection == null)
            {
                createConnection();
            }
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
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
                else if (info.Name.Equals("Dentist"))
                {
                    User activeUser = (User)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, activeUser.No);
                }
                else if (info.Name.Equals("Appointment"))
                {
                    Appointment appointment = (Appointment)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, appointment.No);
                }
                else if (info.Name.Equals("Tooth"))
                {
                    Tooth tooth = (Tooth)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, tooth.No);
                }
                else if (info.Name.Equals("Supplier"))
                {
                    Supplier supplier = (Supplier)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, supplier.No);
                }
                else if (info.Name.Equals("Patient") || info.Name.Equals("Owner"))
                {
                    Patient patient = (Patient)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, patient.No);
                }
                else if (info.Name.Equals("Treatment"))
                {
                    Treatment treatment = (Treatment)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, treatment.No);
                }
                else if (!info.Name.Equals("No") && !info.Name.Equals("DateAdded") && !info.Name.Equals("DateModified"))
                {
                    command.Parameters.AddWithValue("@" + info.Name, info.GetValue(model));
                }
            }
            command.ExecuteNonQuery();
            Connection.Close();
        }

        protected void deleteFromDatabase(T model, string tableName)
        {
            string prefix = new T().GetType().Name;
            if (Connection == null)
            {
                createConnection();
            }
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "DELETE FROM " + tableName + " WHERE "+prefix+"_No=@key";
            command.Parameters.AddWithValue("@key", model.GetType().GetProperty("No").GetValue(model));
            command.ExecuteNonQuery();
            Connection.Close();
        }

        protected void updateDatabase(T model, string tableName)
        {
            string prefix = new T().GetType().Name;
            if(Connection==null)
            {
                createConnection();
            }
            if(Connection.State!=System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "UPDATE " + tableName + " SET ";
            int no = 0;
            foreach (PropertyInfo info in model.GetType().GetProperties())
            {
                if(info.Name.Equals("No"))
                {
                    no = (int)info.GetValue(model);
                }
                if((!info.Name.Equals("No")) && (!info.Name.Equals("DateAdded")) && (!info.Name.Equals("DateModified")))
                {
                    command.CommandText += prefix+"_"+info.Name+"=@" + info.Name + ",";
                }
            }
            command.CommandText = command.CommandText.Remove(command.CommandText.Length - 1) + " WHERE "+prefix+"_No=@key";
            command.Parameters.AddWithValue("@key", no);
            foreach (PropertyInfo info in model.GetType().GetProperties())
            {
                if (info.Name.Equals("AddedBy"))
                {
                    command.Parameters.AddWithValue("@" + info.Name, ActiveUser.No);
                }
                else if (info.Name.Equals("Dentist"))
                {
                    User activeUser = (User)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, activeUser.No);
                }
                else if (info.Name.Equals("Supplier"))
                {
                    Supplier supplier = (Supplier)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, supplier.No);
                }
                else if (info.Name.Equals("Patient") || info.Name.Equals("Owner"))
                {
                    Patient patient = (Patient)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, patient.No);
                }
                else if (info.Name.Equals("Treatment"))
                {
                    Treatment treatment = (Treatment)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, treatment.No);
                }
                else if (!info.Name.Equals("No") && !info.Name.Equals("DateAdded") && !info.Name.Equals("DateModified"))
                {
                    command.Parameters.AddWithValue("@" + info.Name, info.GetValue(model));
                }
            }
            command.ExecuteNonQuery();
            Connection.Close();
        }

        protected object loadInfo(object model, string prefix, string tableName, int key=0)
        {
            if (Connection == null)
            {
                createConnection();
            }
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
            MySqlCommand command = Connection.CreateCommand();
            if(tableName.Equals("allaboutteeth_DentalCharts"))
            {
                tableName = "allaboutteeth_tooths";
            }
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
                    loadInfo(info, info.GetType().Name, "allaboutteeth_" + info.GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), (int)info.GetType().GetProperty("No").GetValue(info));
                }
                return model;
            }
            reader.Close();
            Connection.Close();
            return null;
        }

        protected T loadItem(string tableName, int key=0)
        {
            string prefix = new T().GetType().Name;
            if (Connection == null)
            {
                createConnection();
            }
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM " + tableName+" WHERE "+prefix+"_No=@no";
            command.Parameters.AddWithValue("@no", key);
            List<object> temp = new List<object>();
            MySqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                T model = new T();
                foreach(PropertyInfo info in model.GetType().GetProperties())
                {
                    if(info.PropertyType.ToString().Equals("System.String"))
                    {
                        info.SetValue(model, reader.GetString(prefix + "_" + info.Name));
                    }
                    else if(info.PropertyType.ToString().Equals("System.Int32"))
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
                    loadInfo(info, info.GetType().Name, "allaboutteeth_" + info.GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), (int)info.GetType().GetProperty("No").GetValue(info));
                }
                return model;
            }
            reader.Close();
            Connection.Close();
            return default(T);
        }

        private string tableName = "";
        private string filter = "";
        private T model;

        protected void startSaveToDatabase(T model, string tableName)
        {
            if (addThread == null || !addThread.IsAlive)
            {
                this.tableName = tableName;
                this.model = model;
                addThread = new Thread(executeSaveToDatabase);
                addThread.IsBackground = true;
                addThread.Start();
            }
        }

        private void executeSaveToDatabase()
        {
            if (beforeSave())
            {
                try
                {
                    saveToDatabase(model, tableName);
                    afterSave(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    afterSave(false);
                }
            }
        }

        protected void startUpdateToDatabase(T model, string tableName)
        {
            if (saveThread == null || !saveThread.IsAlive)
            {
                this.tableName = tableName;
                this.model = model;
                saveThread = new Thread(executeUpdateToDatabase);
                saveThread.IsBackground = true;
                saveThread.Start();
            }
        }

        private void executeUpdateToDatabase()
        {
            if(beforeUpdate())
            {
                try
                {
                    updateDatabase(model, tableName);
                    afterUpdate(true);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    afterUpdate(false);
                }
            }
        }

        protected void startDeleteFromDatabase(T model, string tableName)
        {
            if (deleteThread == null || !deleteThread.IsAlive)
            {
                this.tableName = tableName;
                this.model = model;
                deleteThread = new Thread(executeDeleteFromDatabase);
                deleteThread.IsBackground = true;
                deleteThread.Start();
            }
        }

        private void executeDeleteFromDatabase()
        {
            if (beforeDelete())
            {
                try
                {
                    deleteFromDatabase(model, tableName);
                    afterDelete(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    afterDelete(false);
                }
            }

        }

        protected void startLoadFromDatabase(string tableName, string filter)
        {
            if(loadThread==null||!loadThread.IsAlive)
            {
                this.tableName = tableName;
                this.filter = filter;
                loadThread = new Thread(executeLoadFromDatabase);
                loadThread.IsBackground = true;
                loadThread.Start();
            }
        }

        private void executeLoadFromDatabase()
        {
            List<T> list = loadFromDatabase(tableName, filter);
            setLoaded(list);
        }

        protected abstract void setLoaded(List<T> list);
        protected abstract bool beforeUpdate();
        protected abstract void afterUpdate(bool isSuccessful);
        protected abstract bool beforeSave();
        protected abstract void afterSave(bool isSuccessful);
        protected abstract bool beforeDelete();
        protected abstract void afterDelete(bool isSuccessful);

        private double progress = 0;
        public double Progress { get => progress; set { progress = value; OnPropertyChanged(); } }

        public string CustomFilter { get => customFilter; set => customFilter = value; }

        private string customFilter = "1";

        protected List<T> loadFromDatabase(string tableName, string filter)
        {
            Progress = 0;
            string prefix = new T().GetType().Name;
            List<T> list = new List<T>();
            List<int> primaryKeys = new List<int>();
            if(Connection==null)
            {
                createConnection();
            }
            if(Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM "+tableName;
            if(!filter.Trim().Equals(""))
            {
                command.CommandText += " WHERE";
                foreach(PropertyInfo info in new T().GetType().GetProperties())
                {
                    command.CommandText += " " + prefix + "_" + info.Name+" LIKE @"+info.Name+" OR ";
                }
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 3);
                foreach (PropertyInfo info in new T().GetType().GetProperties())
                {
                    command.Parameters.AddWithValue("@"+info.Name,"%"+filter+"%");
                }
            }
            else if(!CustomFilter.Equals("1"))
            {
                command.CommandText +=" WHERE " + CustomFilter;
            }
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                primaryKeys.Add(reader.GetInt32(prefix+"_No"));
            }
            reader.Close();
            Connection.Close();
            foreach(int primaryKey in primaryKeys)
            {
                list.Add(loadItem(tableName, primaryKey));
                Progress = ((double)list.Count / primaryKeys.Count)*100;
            }
            return list;
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

        protected string validateUsername(string value, string original)
        {
            try
            {
                if (Connection == null)
                {
                    createConnection();
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
                    createConnection();
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
