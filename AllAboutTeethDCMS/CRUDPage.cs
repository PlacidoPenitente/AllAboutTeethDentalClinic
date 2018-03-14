using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS
{
    public class CRUDPage<T> : PageViewModel where T : new()
    {
        protected void saveToDatabase(T model, string tableName)
        {
            createConnection();
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "INSERT INTO "+tableName+" VALUES (";
            foreach(PropertyInfo info in model.GetType().GetProperties())
            {
                if(info.Name.Equals("No"))
                {
                    command.CommandText += "NULL,";
                }
                else if(info.Name.Equals("DateAdded") || info.Name.Equals("DateModified"))
                {
                    command.CommandText += "NOW(),";
                }
                else
                {
                    command.CommandText += "@"+info.Name+",";
                }
            }
            command.CommandText = command.CommandText.Remove(command.CommandText.Length-1)+");";

            foreach (PropertyInfo info in model.GetType().GetProperties())
            {
                if(info.Name.Equals("AddedBy"))
                {
                    User activeUser = (User)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, activeUser.No);
                }
                else if(!info.Name.Equals("No") && !info.Name.Equals("DateAdded") && !info.Name.Equals("DateModified"))
                {
                    command.Parameters.AddWithValue("@"+info.Name,info.GetValue(model));
                }
            }
            command.ExecuteNonQuery();
            Connection.Close();
        }

        protected void deleteFromDatabase(T model, string tableName)
        {
            string prefix = new T().GetType().Name;
            createConnection();
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "DELETE FROM " + tableName + " WHERE "+prefix+"_No=@key";
            command.Parameters.AddWithValue("@key", model.GetType().GetProperty("No").GetValue(model));
            command.ExecuteNonQuery();
            Connection.Close();
        }

        protected void updateDatabase(T model, string tableName)
        {
            string prefix = new T().GetType().Name;
            createConnection();
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
                    User activeUser = (User)info.GetValue(model);
                    command.Parameters.AddWithValue("@" + info.Name, activeUser.No);
                }
                else if (!info.Name.Equals("No") && !info.Name.Equals("DateAdded") && !info.Name.Equals("DateModified"))
                {
                    command.Parameters.AddWithValue("@" + info.Name, info.GetValue(model));
                }
            }
            command.ExecuteNonQuery();
            Connection.Close();
        }

        protected object loadInfo(PropertyInfo propertyInfo, string prefix, string tableName, int key=0)
        {
            object model = Activator.CreateInstance(propertyInfo.DeclaringType);
            createConnection();
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM " + tableName + " WHERE " + prefix + "_No=@no";
            command.Parameters.AddWithValue("@no", key);
            Console.WriteLine(tableName);
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
                    else
                    {
                        info.SetValue(model, loadInfo(info, info.PropertyType.Name, "allaboutteeth_" + info.PropertyType.Namespace.Replace("AllAboutTeethDCMS.", ""), reader.GetInt32(prefix + "_" + info.Name)));
                    }
                }
                reader.Close();
                Connection.Close();
                return model;
            }
            reader.Close();
            Connection.Close();
            return null;
        }

        protected T loadItem(string tableName, int key=0)
        {
            string prefix = new T().GetType().Name;
            createConnection();
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM " + tableName+" WHERE "+prefix+"_No=@no";
            command.Parameters.AddWithValue("@no", key);
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
                    else
                    {
                        info.SetValue(model, loadInfo(info, info.PropertyType.Name, "allaboutteeth_" + info.PropertyType.Namespace.Replace("AllAboutTeethDCMS.", ""), reader.GetInt32(prefix + "_" + info.Name)));
                    }
                }
                reader.Close();
                Connection.Close();
                return model;
            }
            reader.Close();
            Connection.Close();
            return default(T);
        }

        protected List<T> loadFromDatabase(string tableName, string filter)
        {
            string prefix = new T().GetType().Name;
            List<T> list = new List<T>();
            List<int> primaryKeys = new List<int>();
            createConnection();
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
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                primaryKeys.Add(reader.GetInt32(prefix+"_No"));
            }
            reader.Close();
            Connection.Close();
            foreach(int primaryKey in primaryKeys)
            {
                T model = new T();
                list.Add(loadItem(tableName, primaryKey));
            }
            return list;
        }
    }
}
