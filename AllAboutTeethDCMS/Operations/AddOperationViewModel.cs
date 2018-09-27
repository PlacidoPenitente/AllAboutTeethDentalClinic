using AllAboutTeethDCMS.ActivityLogs;
using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.Billings;
using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Medicines;
using AllAboutTeethDCMS.Providers;
using AllAboutTeethDCMS.TreatmentRecords;
using AllAboutTeethDCMS.UsedItems;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace AllAboutTeethDCMS.Operations
{
    public class AddOperationViewModel : CRUDPage<Operation>
    {
        private Operation operation;
        private Operation copyOperation;
        private DentalChartViewModel dentalChartViewModel;
        private MedicineViewModel medicineViewModel;
        private AddBillingViewModel addBillingViewModel;

        public DelegateCommand AllPermanentCommand { get; set; }
        public DelegateCommand UpperPermanentCommand { get; set; }
        public DelegateCommand LowerPermanentCommand { get; set; }
        public DelegateCommand AllTemporaryCommand { get; set; }
        public DelegateCommand UpperTemporaryCommand { get; set; }
        public DelegateCommand LowerTemporaryCommand { get; set; }
        public DelegateCommand DeselectCommand { get; set; }

        public void AllPermanent()
        {
            DentalChartViewModel.TeethView.Clear();
            ToothList = null;
            ToothList = new List<ToothViewModel>();
            foreach (var toothViewModel in DentalChartViewModel.AllTooth)
            {
                toothViewModel.IsSelected = false;
                if (toothViewModel.IsAllowed)
                {
                    SelectUpperPermanent(toothViewModel);
                    SelectLowerPermanent(toothViewModel);
                }
            }
        }

        public void UpperPermanent()
        {
            DentalChartViewModel.TeethView.Clear();
            ToothList = null;
            ToothList = new List<ToothViewModel>();
            foreach (var toothViewModel in DentalChartViewModel.AllTooth)
            {
                toothViewModel.IsSelected = false;
                if (toothViewModel.IsAllowed)
                {
                    SelectUpperPermanent(toothViewModel);
                }
            }
        }

        public void LowerPermanent()
        {
            DentalChartViewModel.TeethView.Clear();
            ToothList = null;
            ToothList = new List<ToothViewModel>();
            foreach (var toothViewModel in DentalChartViewModel.AllTooth)
            {
                toothViewModel.IsSelected = false;
                if (toothViewModel.IsAllowed)
                {
                    SelectLowerPermanent(toothViewModel);
                }
            }
        }

        public void AllTemporary()
        {
            DentalChartViewModel.TeethView.Clear();
            ToothList = null;
            ToothList = new List<ToothViewModel>();
            foreach (var toothViewModel in DentalChartViewModel.AllTooth)
            {
                toothViewModel.IsSelected = false;
                if (toothViewModel.IsAllowed)
                {
                    SelectUpperTemporary(toothViewModel);
                    SelectLowerTemporary(toothViewModel);
                }
            }
        }

        public void UpperTemporary()
        {
            DentalChartViewModel.TeethView.Clear();
            ToothList = null;
            ToothList = new List<ToothViewModel>();
            foreach (var toothViewModel in DentalChartViewModel.AllTooth)
            {
                toothViewModel.IsSelected = false;
                if (toothViewModel.IsAllowed)
                {
                    SelectUpperTemporary(toothViewModel);
                }
            }
        }

        public void LowerTemporary()
        {
            DentalChartViewModel.TeethView.Clear();
            ToothList = null;
            ToothList = new List<ToothViewModel>();
            foreach (var toothViewModel in DentalChartViewModel.AllTooth)
            {
                toothViewModel.IsSelected = false;
                if (toothViewModel.IsAllowed)
                {
                    SelectLowerTemporary(toothViewModel);
                }
            }
        }

        private void SelectUpperTemporary(ToothViewModel toothViewModel)
        {
            var no = int.Parse(toothViewModel.ToothNo);
            if (no < 56 && no > 50)
            {
                toothViewModel.IsSelected = true;
                toothViewModel.Teeth.Add(toothViewModel);
            }
            else if (no < 66 && no > 60)
            {
                toothViewModel.Teeth.Add(toothViewModel);
                toothViewModel.IsSelected = true;
            }
        }

        public void SelectLowerTemporary(ToothViewModel toothViewModel)
        {
            var no = int.Parse(toothViewModel.ToothNo);
            if (no < 76 && no > 70)
            {
                toothViewModel.Teeth.Add(toothViewModel);
                toothViewModel.IsSelected = true;
            }
            else if (no < 86 && no > 80)
            {
                toothViewModel.Teeth.Add(toothViewModel);
                toothViewModel.IsSelected = true;
            }
        }

        private void SelectLowerPermanent(ToothViewModel toothViewModel)
        {
            var no = int.Parse(toothViewModel.ToothNo);
            if (no < 49 && no > 40)
            {
                toothViewModel.IsSelected = true;
                toothViewModel.Teeth.Add(toothViewModel);
            }
            else if (no < 39 && no > 30)
            {
                toothViewModel.Teeth.Add(toothViewModel);
                toothViewModel.IsSelected = true;
            }
        }

        public void SelectUpperPermanent(ToothViewModel toothViewModel)
        {
            var no = int.Parse(toothViewModel.ToothNo);
            if (no < 19 && no > 10)
            {
                toothViewModel.Teeth.Add(toothViewModel);
                toothViewModel.IsSelected = true;
            }
            else if (no < 29 && no > 20)
            {
                toothViewModel.Teeth.Add(toothViewModel);
                toothViewModel.IsSelected = true;
            }
        }

        private string amountCharge = "0";

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
            if (ToothList != null && ToothList.Count > 0)
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
                    foreach (ConsumableItem consumable in Consumables)
                    {
                        AddUsedItemViewModel addUsedItemViewModel = new AddUsedItemViewModel();
                        addUsedItemViewModel.Appointment = Appointment;
                        addUsedItemViewModel.Medicine = consumable.Medicine;
                        addUsedItemViewModel.Quantity = Int32.Parse(consumable.Consumed);
                        addUsedItemViewModel.ActiveUser = ActiveUser;
                        consumable.ViewModel.ActiveUser = ActiveUser;
                        consumable.consume();
                        addUsedItemViewModel.SaveUsedItem();
                    }
                    AddBillingViewModel.Billing.Provider = ProviderViewModel.Provider;
                    if (AddBillingViewModel.Billing.Provider == null)
                    {
                        AddBillingViewModel.Billing.Provider = new Provider();
                    }
                    AddBillingViewModel.ActiveUser = ActiveUser;
                    AddBillingViewModel.Appointment = Appointment;
                    AddBillingViewModel.Billing.AmountCharged = Double.Parse(AmountCharge);
                    AddBillingViewModel.Billing.Balance = Double.Parse(AmountCharge);
                    AddBillingViewModel.saveBilling();



                    MessageBox.Show("Treatment was successfully saved.", "Treatment Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    MySqlConnection connection = CreateConnection();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "update allaboutteeth_appointments set appointment_status='Completed' where appointment_no = '" + Appointment.No + "'";
                    command.ExecuteNonQuery();
                    connection.Close();
                    connection = null;

                    MySqlConnection connection2 = CreateConnection();
                    MySqlCommand command2 = connection2.CreateCommand();
                    command2.CommandText = "update allaboutteeth_patients set patient_status='Active', patient_addedby='" + ActiveUser.No + "' where patient_no='" + Appointment.Patient.No + "'";
                    command2.ExecuteNonQuery();
                    connection2.Close();
                    connection2 = null;

                    AddActivityLogViewModel addActivityLog = new AddActivityLogViewModel();
                    addActivityLog.ActivityLog = new ActivityLog();
                    addActivityLog.ActiveUser = ActiveUser;
                    addActivityLog.ActivityLog.Activity = "User performed treatment for " + Operation.Appointment.Patient.FirstName + " " + Operation.Appointment.Patient.LastName + ".";
                    addActivityLog.saveActivityLog();

                    try
                    {
                        var appointment = Appointments.OrderBy(x=>x.No).First(x =>
                            x.Patient.No == Appointment.Patient.No && x.No != Appointment.No);
                        Appointment = appointment;
                        Appointments.Remove(appointment);
                        Teeth = new List<ToothViewModel>();
                        ToothList = new List<ToothViewModel>();
                        ClearItems();
                    }
                    catch (Exception)
                    {
                        MenuViewModel.gotoBillings();
                    }
                    //MenuViewModel.gotoBillings();
                }
            }
            else
            {
                MessageBox.Show("No tooth is selected.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ProviderViewModel providerViewModel;

        public AddOperationViewModel()
        {
            operation = new Operation();
            dentalChartViewModel = new DentalChartViewModel()
            {
                TreatmentRecordViewModel = null
            };
            copyOperation = (Operation)operation.Clone();

            AddCommand = new DelegateCommand(new Action(addTooth));
            RemoveCommand = new DelegateCommand(new Action(removeTooth));
            ClearCommand = new DelegateCommand(new Action(clearTeeth));

            MedicineViewModel = new MedicineViewModel();
            MedicineViewModel.LoadMedicines();

            ProviderViewModel = new ProviderViewModel();
            ProviderViewModel.LoadProviders();

            AddItemCommand = new DelegateCommand(AddItem);
            RemoveItemCommand = new DelegateCommand(RemoveItem);
            ClearItemsCommad = new DelegateCommand(ClearItems);

            Consumables = new List<ConsumableItem>();
            AddBillingViewModel = new AddBillingViewModel();

            AllPermanentCommand = new DelegateCommand(AllPermanent);
            UpperPermanentCommand = new DelegateCommand(UpperPermanent);
            LowerPermanentCommand = new DelegateCommand(LowerPermanent);
            AllTemporaryCommand = new DelegateCommand(AllTemporary);
            UpperTemporaryCommand = new DelegateCommand(UpperTemporary);
            LowerTemporaryCommand = new DelegateCommand(LowerTemporary);
            DeselectCommand = new DelegateCommand(Deselect);
        }

        private void Deselect()
        {
            DentalChartViewModel.TeethView.Clear();
            ToothList = null;
            ToothList = new List<ToothViewModel>();
            foreach (var toothViewModel in DentalChartViewModel.AllTooth)
            {
                toothViewModel.IsSelected = false;
            }
        }

        public void initialize()
        {
            MedicineViewModel.ActiveUser = ActiveUser;
            ProviderViewModel.ActiveUser = ActiveUser;
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

        //public Tooth Tooth { get => Operation.Tooth; set { Operation.Tooth = value; OnPropertyChanged(); } }
        //public Treatment Treatment { get => Operation.Treatment; set { Operation.Treatment = value; OnPropertyChanged(); } }
        //public double AmountCharged { get => Operation.AmountCharged; set { Operation.AmountCharged = value; OnPropertyChanged(); } }
        //public double AmountPaid { get => Operation.AmountPaid; set { Operation.AmountPaid = value; OnPropertyChanged(); } }
        //public double Balance { get => Operation.Balance; set { Operation.Balance = value; OnPropertyChanged(); } }
        public Appointment Appointment
        {
            get => Operation.Appointment; 
            set
            {
                var appointment = Appointments.OrderBy(x=>x.No).First(x =>
                    x.Patient.No == value.Patient.No);
                Appointments.Remove(appointment);

                Operation.Appointment = appointment; 
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
                DentalChartViewModel.TeethView.Clear();
                var temp = DentalChartViewModel.TreatmentRecordViewModel;
                DentalChartViewModel = new DentalChartViewModel();
                DentalChartViewModel.TreatmentRecordViewModel = temp;
                DentalChartViewModel.User = ActiveUser;
                DentalChartViewModel.Treatment = Appointment.Treatment;
                DentalChartViewModel.Patient = appointment.Patient;
                Teeth = DentalChartViewModel.TeethView;
            }
        }

        public DentalChartViewModel DentalChartViewModel { get => dentalChartViewModel; set { dentalChartViewModel = value; OnPropertyChanged(); } }

        public List<ToothViewModel> Teeth { get => teeth; set { teeth = value; OnPropertyChanged(); } }

        public List<ToothViewModel> ToothList { get => toothList; set { toothList = value; OnPropertyChanged(); } }

        public DelegateCommand AddCommand { get => addCommand; set => addCommand = value; }
        public DelegateCommand RemoveCommand { get => removeCommand; set => removeCommand = value; }
        public DelegateCommand ClearCommand { get => clearCommand; set => clearCommand = value; }
        public ToothViewModel SelectedTVM { get => selectedTVM; set { selectedTVM = value; OnPropertyChanged(); } }

        public MedicineViewModel MedicineViewModel { get => medicineViewModel; set => medicineViewModel = value; }
        public DelegateCommand AddItemCommand { get => addItemCommand; set => addItemCommand = value; }
        public DelegateCommand RemoveItemCommand { get => removeItemCommand; set => removeItemCommand = value; }
        public DelegateCommand ClearItemsCommad { get => clearItemsCommad; set => clearItemsCommad = value; }
        public List<ConsumableItem> Consumables { get => consumables; set { consumables = value; OnPropertyChanged(); } }
        public List<Medicine> SelectedItems { get => selectedItems; set => selectedItems = value; }
        public ConsumableItem SelectedConsumable { get => selectedConsumable; set { selectedConsumable = value; OnPropertyChanged(); } }

        public AddBillingViewModel AddBillingViewModel { get => addBillingViewModel; set => addBillingViewModel = value; }
        public string AmountCharge
        {
            get => amountCharge;
            set
            {
                if (!value.Contains(" "))
                {
                    try
                    {
                        double amount = Double.Parse(value);
                        if (amount > -1)
                        {
                            amountCharge = value;
                            if (amount > 0)
                            {
                                ProviderViewModel.Provider = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                OnPropertyChanged();
            }
        }

        public ProviderViewModel ProviderViewModel { get => providerViewModel; set => providerViewModel = value; }
        public List<Appointment> Appointments { get; set; }

        private List<ToothViewModel> toothList;

        private DelegateCommand addCommand;
        private DelegateCommand removeCommand;
        private DelegateCommand clearCommand;

        public void addTooth()
        {
            List<ToothViewModel> temp = new List<ToothViewModel>();
            if (ToothList != null)
            {
                temp.AddRange(ToothList);
            }
            foreach (ToothViewModel tvm in DentalChartViewModel.TeethView)
            {
                if (!temp.Contains(tvm))
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
            if (SelectedTVM != null)
            {
                temp.Remove(SelectedTVM);
                SelectedTVM = null;
            }
            ToothList = temp;
        }

        public void clearTeeth()
        {
            ToothList = null;
            ToothList = new List<ToothViewModel>();
        }

        private DelegateCommand addItemCommand;
        private DelegateCommand removeItemCommand;
        private DelegateCommand clearItemsCommad;

        private List<ConsumableItem> consumables;
        private ConsumableItem selectedConsumable;
        private List<Medicine> selectedItems;

        public void AddItem()
        {
            if (MedicineViewModel.Medicine != null)
            {
                if (SelectedItems == null)
                {
                    SelectedItems = new List<Medicine>();
                }
                if (!SelectedItems.Contains(MedicineViewModel.Medicine))
                {
                    if (MedicineViewModel.Medicine.Quantity > 0)
                    {
                        SelectedItems.Add(MedicineViewModel.Medicine);
                        List<ConsumableItem> temp = new List<ConsumableItem>();
                        temp.AddRange(Consumables);
                        temp.Add(new ConsumableItem()
                        {
                            Medicine = MedicineViewModel.Medicine,
                            Consumed = "0"
                        }
                        );
                        Consumables = temp;
                    }
                }
            }
        }

        public void RemoveItem()
        {
            if (SelectedConsumable != null)
            {
                List<ConsumableItem> temp = new List<ConsumableItem>();
                temp.AddRange(Consumables);
                temp.Remove(SelectedConsumable);
                SelectedItems.Remove(SelectedConsumable.Medicine);
                Consumables = temp;
            }
            SelectedConsumable = null;
        }

        public void ClearItems()
        {
            if (SelectedItems != null)
            {
                SelectedItems.Clear();
                Consumables = null;
                Consumables = new List<ConsumableItem>();
            }
        }
    }
}
