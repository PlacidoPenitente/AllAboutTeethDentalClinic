using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class EditUserViewModel : AddUserViewModel
    {
        public override void saveUser()
        {
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                info.SetValue(this, info.GetValue(this));
            }
            bool hasError = false;
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                if (info.Name.EndsWith("Error"))
                {
                    if (!((string)info.GetValue(this)).Equals(""))
                    {
                        hasError = true;
                        break;
                    }
                    else
                    {
                        DialogBoxViewModel.Mode = "Error";
                        DialogBoxViewModel.Title = "Save Failed";
                        DialogBoxViewModel.Message = "Form contains errors. Please check all required fields.";
                        DialogBoxViewModel.Answer = "None";
                    }
                }
            }
            if (!hasError)
            {
                startUpdateToDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        public override void startResetThread()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Confirm";
            DialogBoxViewModel.Title = "Reset Form";
            DialogBoxViewModel.Message = "Resetting form will restore previous values. Proceed?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("OK"))
            {
                User = (User)CopyUser.Clone();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    if (info.Name.EndsWith("Error"))
                    {
                        info.SetValue(this, "");
                    }
                }
                PasswordCopy = "";
            }
            DialogBoxViewModel.Answer = "";
        }
    }
}
