using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Appointments
{
    public class EditAppointmentViewModel : AddAppointmentViewModel
    {
        public override void saveAppointment()
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
                }
            }
            if (!hasError)
            {
                startUpdateToDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        public override void startResetThread()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Reset Form";
            DialogBoxViewModel.Message = "Are you sure you want to reset this form?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                Appointment = (Appointment)CopyAppointment.Clone();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    if (info.Name.EndsWith("Error"))
                    {
                        info.SetValue(this, "");
                    }
                }
            }
            DialogBoxViewModel.Answer = "";
        }
    }
}
