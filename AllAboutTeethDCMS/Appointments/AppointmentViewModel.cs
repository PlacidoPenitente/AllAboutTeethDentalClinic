using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Appointments
{
    public class AppointmentViewModel : CRUDPage<Appointment>
    {
        private Appointment appointment;
        private List<Appointment> appointments;
        private string filter = "";

        private DialogBoxViewModel dialogBoxViewModel;
        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        private string filterResult = "";

        public DialogBoxViewModel DialogBoxViewModel { get => dialogBoxViewModel; set { dialogBoxViewModel = value; OnPropertyChanged(); } }
        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }
        public string FilterResult { get => filterResult; set { filterResult = value; OnPropertyChanged(); } }

        public void archive()
        {
            startUpdateToDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void unarchive()
        {
            startUpdateToDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Appointment.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Appointment";
                DialogBoxViewModel.Message = "Are you sure you want to archive this appointment? Appointment can no longer be used.";
            }
            else
            {
                DialogBoxViewModel.Title = "Unarchive Appointment";
                DialogBoxViewModel.Message = "Are you sure you want to activate this appointment?";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                if (Appointment.Status.Equals("Active"))
                {
                    Appointment.Status = "Archived";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Archiving appointment. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                else
                {
                    Appointment.Status = "Active";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Activating appointment. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                return true;
            }
            return false;
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                loadAppointments();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        protected override bool beforeDelete()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Delete Appointment";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this appointment?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleteing appointment. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                loadAppointments();
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        public AppointmentViewModel()
        {
            DialogBoxViewModel = new DialogBoxViewModel();
        }

        public Appointment Appointment
        {
            get => appointment; set
            {
                appointment = value; OnPropertyChanged();
                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                if (value != null)
                {
                    if (value.Status.Equals("Active"))
                    {
                        ArchiveVisibility = "Visible";
                    }
                    else
                    {
                        UnarchiveVisibility = "Visible";
                    }
                }
            }
        }



        public List<Appointment> Appointments { get => appointments; set { appointments = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadAppointments(); OnPropertyChanged(); } }

        public void loadAppointments()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteAppointment()
        {
            startDeleteFromDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Appointment> list)
        {
            Appointments = list;
            FilterResult = "";
            if (list.Count > 0)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }
        }

        protected override bool beforeSave()
        {
            return true;
        }

        protected override void afterSave(bool isSuccessful)
        {
            throw new NotImplementedException();
        }
    }
}
