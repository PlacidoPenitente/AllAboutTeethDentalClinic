using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class EditUserViewModel : AddUserViewModel
    {
        public override void saveUser()
        {
            User.AddedBy = ActiveUser;
            updateDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public override void resetForm()
        {
            User = (User)CopyUser.Clone();
        }
    }
}
