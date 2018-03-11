using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class User : ICloneable
    {
        private int no = -1;
        private string username = "";
        private string password = "";
        private string type = "Staff";
        private string firstName = "";
        private string lastName = "";
        private string middleName = "";
        private DateTime birthdate = DateTime.Now;
        private string gender = "Male";
        private string address = "";
        private string contactNo = "";
        private string emailAddress = "";
        private string question1 = "";
        private string question2 = "";
        private string answer1 = "";
        private string answer2 = "";
        private string image = "";
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;

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
        public string Image { get => image; set => image = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }

        public object Clone()
        {
            var clone = Activator.CreateInstance(GetType());
            PropertyInfo[] propertyInfos = clone.GetType().GetProperties();
            for(int i=0; i<GetType().GetProperties().Count(); i++)
            {
                propertyInfos[i].SetValue(clone, GetType().GetProperties().ElementAt(i).GetValue(this));
            }
            return clone;
        }

        public string validate([CallerMemberName] String propertyName = null)
        {
            string error = "";
            return error;
        }
    }
}
