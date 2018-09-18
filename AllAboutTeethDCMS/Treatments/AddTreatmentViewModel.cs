using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AllAboutTeethDCMS.ActivityLogs;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Treatments
{
    public class AddTreatmentViewModel : CRUDPage<Treatment>
    {
        private Treatment treatment;
        private Treatment copyTreatment;

        public AddTreatmentViewModel() : base()
        {
            treatment = new Treatment();
            copyTreatment = (Treatment)treatment.Clone();
            Outputs = new List<string>()
            {
                "None",
                "Decayed (Caries Indicated For Filling)",
                "Missing Due To Caries",
                "Filled",
                "Caries Indicated For Extraction",
                "Root Fragment",
                "Missing Due To Other Causes",
                "Impacted Tooth",
                "Jacket Crown",
                "Amalgam Filling",
                "Abutment",
                "Pontic",
                "Inlay",
                "Fixed Cure Composite",
                "Removable Denture",
                "Extraction Due To Caries",
                "Extraction Due To Other Causes",
                "Present Teeth",
                "Congenitally Missing",
                "Supernumerary"
            };
        }
        protected override bool beforeCreate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Add Service";
            DialogBoxViewModel.Message = "Are you sure you want to add this service?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Adding serivce. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterCreate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                Treatment = new Treatment();
                CopyTreatment = (Treatment)Treatment.Clone();

                AddActivityLogViewModel addActivityLog = new AddActivityLogViewModel();
                addActivityLog.ActivityLog = new ActivityLog();
                addActivityLog.ActiveUser = ActiveUser;
                addActivityLog.ActivityLog.Activity = "User created a new service named " + Treatment.Name + ".";
                addActivityLog.saveActivityLog();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
            }
        }

        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Update Service";
            DialogBoxViewModel.Message = "Are you sure you want to update this service?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Updating service. Please wait.";
                DialogBoxViewModel.Answer = "None";
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
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                CopyTreatment = (Treatment)Treatment.Clone();

                AddActivityLogViewModel addActivityLog = new AddActivityLogViewModel();
                addActivityLog.ActivityLog = new ActivityLog();
                addActivityLog.ActiveUser = ActiveUser;
                addActivityLog.ActivityLog.Activity = "User updated service named " + Treatment.Name + ".";
                addActivityLog.saveActivityLog();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
            }
        }

        public virtual void saveTreatment()
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
                startSaveToDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Save Failed";
                DialogBoxViewModel.Message = "Form contains errors. Please check all required fields.";
                DialogBoxViewModel.Answer = "None";
            }
        }

        #region Reset Thread

        private Thread resetThread;

        public virtual void startResetThread()
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
                Treatment = new Treatment();
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

        public void resetForm()
        {
            resetThread = new Thread(startResetThread);
            resetThread.IsBackground = true;
            resetThread.Start();
        }
        #endregion

        #region Unimplemented Methods
        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<Treatment> list)
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
        #endregion

        public Treatment Treatment { get => treatment; set { treatment = value; foreach (PropertyInfo info in GetType().GetProperties()) OnPropertyChanged(info.Name); } }
        public Treatment CopyTreatment { get => copyTreatment; set { copyTreatment = value; } }

        private string nameError = "";
        public string NameError { get => nameError; set { nameError = value; OnPropertyChanged(); } }

        private List<string> outputs;
        public List<string> Outputs { get => outputs; set { outputs = value; OnPropertyChanged(); } }

        public string Name
        {
            get => Treatment.Name; set
            {
                if (!value.Contains("  "))
                {
                    Treatment.Name = value;
                    NameError = "";
                    NameError = validateUniqueName(value, CopyTreatment.Name, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", "")); OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get => Treatment.Description;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Treatment.Description = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool DecayedCariesIndicatedForFilling { get => Treatment.DecayedCariesIndicatedForFilling; set { Treatment.DecayedCariesIndicatedForFilling = value; OnPropertyChanged(); } }
        public bool MissingDueToCaries { get => Treatment.MissingDueToCaries; set { Treatment.MissingDueToCaries = value; OnPropertyChanged(); } }
        public bool Filled { get => Treatment.Filled; set { Treatment.Filled = value; OnPropertyChanged(); } }
        public bool CariesIndicatedForExtraction { get => Treatment.CariesIndicatedForExtraction; set { Treatment.CariesIndicatedForExtraction = value; OnPropertyChanged(); } }
        public bool RootFragment { get => Treatment.RootFragment; set { Treatment.RootFragment = value; OnPropertyChanged(); } }
        public bool MissingDueToOtherCauses { get => Treatment.MissingDueToOtherCauses; set { Treatment.MissingDueToOtherCauses = value; OnPropertyChanged(); } }
        public bool ImpactedTooth { get => Treatment.ImpactedTooth; set { Treatment.ImpactedTooth = value; OnPropertyChanged(); } }
        public bool JacketCrown { get => Treatment.JacketCrown; set { Treatment.JacketCrown = value; OnPropertyChanged(); } }
        public bool AmalgamFilling { get => Treatment.AmalgamFilling; set { Treatment.AmalgamFilling = value; OnPropertyChanged(); } }
        public bool Abutment { get => Treatment.Abutment; set { Treatment.Abutment = value; OnPropertyChanged(); } }
        public bool Pontic { get => Treatment.Pontic; set { Treatment.Pontic = value; OnPropertyChanged(); } }
        public bool Inlay { get => Treatment.Inlay; set { Treatment.Inlay = value; OnPropertyChanged(); } }
        public bool FixedCureComposite { get => Treatment.FixedCureComposite; set { Treatment.FixedCureComposite = value; OnPropertyChanged(); } }
        public bool RemovableDenture { get => Treatment.RemovableDenture; set { Treatment.RemovableDenture = value; OnPropertyChanged(); } }
        public bool ExtractionDueToCaries { get => Treatment.ExtractionDueToCaries; set { Treatment.ExtractionDueToCaries = value; OnPropertyChanged(); } }
        public bool ExtractionDueToOtherCauses { get => Treatment.ExtractionDueToOtherCauses; set { Treatment.ExtractionDueToOtherCauses = value; OnPropertyChanged(); } }
        public bool PresentTeeth { get => Treatment.PresentTeeth; set { Treatment.PresentTeeth = value; OnPropertyChanged(); } }
        public bool CongenitallyMissing { get => Treatment.CongenitallyMissing; set { Treatment.CongenitallyMissing = value; OnPropertyChanged(); } }
        public bool Supernumerary { get => Treatment.Supernumerary; set { Treatment.Supernumerary = value; OnPropertyChanged(); } }
        public string Output { get => Treatment.Output; set { Treatment.Output = value; OnPropertyChanged(); } }
    }
}
