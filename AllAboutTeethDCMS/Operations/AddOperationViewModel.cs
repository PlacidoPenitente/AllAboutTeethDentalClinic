using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.TreatmentRecords;
using AllAboutTeethDCMS.Treatments;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Operations
{
    public class AddOperationViewModel : CRUDPage<Operation>
    {
        private Operation operation;
        private Operation copyOperation;
        private DentalChartViewModel dentalChartViewModel;

        public void updateList()
        {
            foreach(ToothViewModel toothViewModel in DentalChartViewModel.TeethView)
            {
                AddTreatmentRecordViewModel treatmentRecord = new AddTreatmentRecordViewModel();
                treatmentRecord.Treatment = Appointment.Treatment;
                treatmentRecord.Appointment = Appointment;
                treatmentRecord.Tooth = toothViewModel.Tooth;
                treatmentRecord.Patient = Appointment.Patient;
                treatmentRecord.ActiveUser = ActiveUser;
                treatmentRecord.saveTreatmentRecord();
            }
        }

        public AddOperationViewModel()
        {
            operation = new Operation();
            dentalChartViewModel = new DentalChartViewModel();
            copyOperation = (Operation)operation.Clone();
        }

        public virtual void resetForm()
        {
            Operation = new Operation();
        }

        public virtual void saveOperation()
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
                Operation.AddedBy = ActiveUser;
                startSaveToDatabase(Operation, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        protected override void setLoaded(List<Operation> list)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate()
        {
            throw new NotImplementedException();
        }

        protected override bool beforeSave()
        {
            throw new NotImplementedException();
        }

        protected override void afterSave()
        {
            throw new NotImplementedException();
        }

        public Operation Operation
        {
            get => operation;
            set
            {
                operation = value;
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }

        private List<ToothViewModel> teeth;

        public Tooth Tooth { get => Operation.Tooth; set { Operation.Tooth = value; OnPropertyChanged(); } }
        public Treatment Treatment { get => Operation.Treatment; set { Operation.Treatment = value; OnPropertyChanged(); } }
        public double AmountCharged { get => Operation.AmountCharged; set { Operation.AmountCharged = value; OnPropertyChanged(); } }
        public double AmountPaid { get => Operation.AmountPaid; set { Operation.AmountPaid = value; OnPropertyChanged(); } }
        public double Balance { get => Operation.Balance; set { Operation.Balance = value; OnPropertyChanged(); } }
        public Appointment Appointment { get => Operation.Appointment; set { Operation.Appointment = value; OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
                DentalChartViewModel.TeethView.Clear();
                DentalChartViewModel = new DentalChartViewModel();
                DentalChartViewModel.User = ActiveUser;
                DentalChartViewModel.Patient = value.Patient;
                Teeth = DentalChartViewModel.TeethView;
            } }

        public DentalChartViewModel DentalChartViewModel { get => dentalChartViewModel; set { dentalChartViewModel = value; OnPropertyChanged(); } }

        public List<ToothViewModel> Teeth { get => teeth; set { teeth = value; OnPropertyChanged(); } }
    }
}
