using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class User : ModelBase
    {
        #region Fields
        private int no = -1;
        private string username = "";
        private string password = "";
        private string type = "Staff";
        private string firstName = "";
        private string middleName = "";
        private string lastName = "";
        private string gender = "Male";
        private DateTime birthdate;
        private string address = "";
        private string contactNo = "";
        private string emailAddress = "";
        private string question1 = "";
        private string question2 = "";
        private string answer1 = "";
        private string answer2 = "";
        private string specialization = "";
        private int rate = 40;
        private string status = "Archived";
        private string image = "";
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        #endregion

        public User()
        {
            if ((DateTime.Now.Year - 18) % 4 != 0 && DateTime.Now.Month == 2 && DateTime.Now.Day == 29)
            {
                Birthdate = DateTime.Parse("2/28/" + (DateTime.Now.Year - 18));
            }
            else
            {
                Birthdate = DateTime.Parse(DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + (DateTime.Now.Year - 18));
            }
        }

        #region Properties
        public int No { get => no; set => no = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Type { get => type; set => type = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string MiddleName { get => middleName; set => middleName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Gender { get => gender; set => gender = value; }
        public DateTime Birthdate { get => birthdate; set => birthdate = value; }
        public string Address { get => address; set => address = value; }
        public string ContactNo { get => contactNo; set => contactNo = value; }
        public string EmailAddress { get => emailAddress; set => emailAddress = value; }
        public string Question1 { get => question1; set => question1 = value; }
        public string Question2 { get => question2; set => question2 = value; }
        public string Answer1 { get => answer1; set => answer1 = value; }
        public string Answer2 { get => answer2; set => answer2 = value; }
        public string Specialization { get => specialization; set => specialization = value; }
        public int Rate { get => rate; set => rate = value; }
        public string Image { get => image; set => image = value; }
        public string Status { get => status; set => status = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        #endregion

        public override string ToString()
        {
            return LastName + ", " + FirstName + " " + MiddleName+ " (" + Username + ") ";
        }
    }
}
