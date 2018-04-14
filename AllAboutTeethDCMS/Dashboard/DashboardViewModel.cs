using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Dashboard
{
    public class DashboardViewModel : PageViewModel
    {
        private int totalPatient = 0;
        private int totalActivePatient = 0;
        private int totalDentist = 0;
        private int totalActiveDentist = 0;
        private int unpaidBills = 0;
        private int scheduled = 0;
        private int critical = 0;
        private int outOfStock = 0;

        public int TotalPatient { get => totalPatient; set { totalPatient = value; OnPropertyChanged(); } }
        public int TotalActivePatient { get => totalActivePatient; set { totalActivePatient = value; OnPropertyChanged(); } }
        public int TotalDentist { get => totalDentist; set { totalDentist = value; OnPropertyChanged(); } }
        public int TotalActiveDentist { get => totalActiveDentist; set { totalActiveDentist = value; OnPropertyChanged(); } }
        public int UnpaidBills { get => unpaidBills; set { unpaidBills = value; OnPropertyChanged(); } }
        public int Scheduled { get => scheduled; set { scheduled = value; OnPropertyChanged(); } }
        public int Critical { get => critical; set { critical = value; OnPropertyChanged(); } }
        public int OutOfStock { get => outOfStock; set { outOfStock = value; OnPropertyChanged(); } }

        public void load()
        {
            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) as 'total' FROM allaboutteeth_database.allaboutteeth_patients";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TotalPatient = reader.GetInt32("total");
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) as 'total' FROM allaboutteeth_database.allaboutteeth_patients where patient_status = 'Active' or patient_status='Scheduled'";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TotalActivePatient = reader.GetInt32("total");
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) as 'total' FROM allaboutteeth_database.allaboutteeth_users where user_type='Dentist' or user_type='Administrator'";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TotalDentist = reader.GetInt32("total");
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) as 'total' FROM allaboutteeth_database.allaboutteeth_users where (user_type='Dentist' or user_type='Administrator') and user_status='Active'";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TotalActiveDentist = reader.GetInt32("total");
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) as total FROM allaboutteeth_database.allaboutteeth_billings where billing_balance<billing_amountcharged";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UnpaidBills = reader.GetInt32("total");
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }

            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) as 'total' FROM allaboutteeth_database.allaboutteeth_patients where patient_status='Scheduled'";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Scheduled = reader.GetInt32("total");
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
        }
    }
}
