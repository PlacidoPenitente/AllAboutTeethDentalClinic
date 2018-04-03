﻿using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Medicines;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.TreatmentRecords;
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

namespace AllAboutTeethDCMS.Operations
{
    public class AddOperationViewModel : CRUDPage<Operation>
    {
        private Operation operation;
        private Operation copyOperation;
        private DentalChartViewModel dentalChartViewModel;
        private MedicineViewModel medicineViewModel;

        public void updateList()
        {
            //ToothList = null;
            //ToothList = DentalChartViewModel.TeethView;
            List<ToothViewModel> temp = new List<ToothViewModel>();
            temp.AddRange(DentalChartViewModel.TeethView);
            ToothList = temp;
        }

        public void finishTreatment()
        {
            if(ToothList!=null && ToothList.Count>0)
            {
                if (MessageBox.Show("Are you sure you want to save this treatment?", "Save Treatment", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (ToothViewModel toothViewModel in ToothList)
                    {
                        AddTreatmentRecordViewModel treatmentRecord = new AddTreatmentRecordViewModel();
                        treatmentRecord.Treatment = Appointment.Treatment;
                        treatmentRecord.Appointment = Appointment;
                        treatmentRecord.Tooth = toothViewModel.Tooth;
                        treatmentRecord.Patient = Appointment.Patient;
                        treatmentRecord.Notes = toothViewModel.Remarks;
                        treatmentRecord.ActiveUser = ActiveUser;
                        treatmentRecord.saveTreatmentRecord();
                        if (!Appointment.Treatment.Output.Equals("None"))
                        {
                            toothViewModel.Tooth.Condition = Appointment.Treatment.Output;
                        }
                        toothViewModel.saveTooth();
                        toothViewModel.loadTooth();
                    }
                    MessageBox.Show("Treatment was successfully saved.", "Treatment Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No tooth is selected.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public AddOperationViewModel()
        {
            operation = new Operation();
            dentalChartViewModel = new DentalChartViewModel();
            copyOperation = (Operation)operation.Clone();

            AddCommand = new DelegateCommand(new Action(addTooth));
            RemoveCommand = new DelegateCommand(new Action(removeTooth));
            ClearCommand = new DelegateCommand(new Action(clearTeeth));

            MedicineViewModel = new MedicineViewModel();
            MedicineViewModel.LoadMedicines();
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

        protected override void afterLoad(List<Operation> list)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeCreate()
        {
            throw new NotImplementedException();
        }

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
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
                DentalChartViewModel.Treatment = Appointment.Treatment;
                DentalChartViewModel.Patient = value.Patient;
                Teeth = DentalChartViewModel.TeethView;
            } }

        public DentalChartViewModel DentalChartViewModel { get => dentalChartViewModel; set { dentalChartViewModel = value; OnPropertyChanged(); } }

        public List<ToothViewModel> Teeth { get => teeth; set { teeth = value; OnPropertyChanged(); } }
 
        public List<ToothViewModel> ToothList { get => toothList; set { toothList = value; OnPropertyChanged(); } }

        public DelegateCommand AddCommand { get => addCommand; set => addCommand = value; }
        public DelegateCommand RemoveCommand { get => removeCommand; set => removeCommand = value; }
        public DelegateCommand ClearCommand { get => clearCommand; set => clearCommand = value; }
        public ToothViewModel SelectedTVM { get => selectedTVM; set { selectedTVM = value; OnPropertyChanged(); } }

        public MedicineViewModel MedicineViewModel { get => medicineViewModel; set => medicineViewModel = value; }

        private List<ToothViewModel> toothList;

        private DelegateCommand addCommand;
        private DelegateCommand removeCommand;
        private DelegateCommand clearCommand;

        public void addTooth()
        {
            List<ToothViewModel> temp = new List<ToothViewModel>();
            if(ToothList!=null)
            {
                temp.AddRange(ToothList);
            }
            foreach(ToothViewModel tvm in DentalChartViewModel.TeethView)
            {
                if(!temp.Contains(tvm))
                {
                    temp.Add(tvm);
                }
            }
            ToothList = temp;
        }

        private ToothViewModel selectedTVM;

        public void removeTooth()
        {
            List<ToothViewModel> temp = new List<ToothViewModel>();
            temp.AddRange(ToothList);
            if(SelectedTVM!=null)
            {
                temp.Remove(SelectedTVM);
                SelectedTVM = null;
            }
            ToothList = temp;
        }

        public void clearTeeth()
        {
            ToothList = new List<ToothViewModel>();
        }
    }
}
