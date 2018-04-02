using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Patients
{
    public class AddPatientViewModel : CRUDPage<Patient>
    {
        private Patient patient;
        private Patient copyPatient;
        private List<string> genders = new List<string>() { "Male", "Female" };


        protected override bool beforeCreate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Add Patient";
            DialogBoxViewModel.Message = "Are you sure you want to add this patient?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Adding patient. Please wait.";
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
                Patient = new Patient();
                CopyPatient = (Patient)Patient.Clone();
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
            DialogBoxViewModel.Title = "Update Patient";
            DialogBoxViewModel.Message = "Are you sure you want to update this patient?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Updating patient. Please wait.";
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
                CopyPatient = (Patient)Patient.Clone();
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

        public virtual void savePatient()
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
                startSaveToDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
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
                Patient = new Patient();
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

        protected override void afterLoad(List<Patient> list)
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
        
public AddPatientViewModel()
        {
            patient = new Patient();
            copyPatient = (Patient)patient.Clone();
        }

        public Patient Patient { get => patient; set { patient = value;
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            } }
        public string LastName { get => Patient.LastName;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.LastName = value;
                    LastNameError = "";
                    LastNameError = validate(value);
                    OnPropertyChanged();
                }

            }
        }
        public string FirstName { get => Patient.FirstName;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.FirstName = value;
                    FirstNameError = "";
                    FirstNameError = validate(value);
                    OnPropertyChanged();
                }

            }
        }
        public string MiddleName { get => Patient.MiddleName;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.MiddleName = value;
                    OnPropertyChanged();
                }

            }
        }
        public DateTime Birthdate { get => Patient.Birthdate; set { Patient.Birthdate = value;
                Occupation = "";
                DentalInsurance = "";
                ParentGuardianName = "";
                OnPropertyChanged(); } }
        public string Sex { get => Patient.Sex; set { Patient.Sex = value; OnPropertyChanged(); } }
        public string Religion { get => Patient.Religion;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Religion = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Nationality { get => Patient.Nationality;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Nationality = value;
                    OnPropertyChanged();
                }

            }
        }
        public string Nickname { get => Patient.Nickname;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Nickname = value;
                    OnPropertyChanged();
                }

            }
        }
        public string HomeAddress { get => Patient.HomeAddress;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.HomeAddress = value;
                    HomeAddressError = "";
                    HomeAddressError = validate(value);
                    OnPropertyChanged();
                }

            }
        }
        public string HomeNo { get => Patient.HomeNo; set {
                bool valid = true;
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    Patient.HomeNo = value;
                }
                OnPropertyChanged(); } }
        public string Occupation { get => Patient.Occupation;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Occupation = value;
                    if (value.Trim().Equals("")) {
                        OfficeNo = "";
                        FaxNo = "";
                    }
                    OnPropertyChanged();
                }

            }
        }
        public string OfficeNo { get => Patient.OfficeNo; set {
                bool valid = true;
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    Patient.OfficeNo = value;
                }
                OnPropertyChanged(); } }
        public string DentalInsurance { get => Patient.DentalInsurance;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.DentalInsurance = value;
                    OnPropertyChanged();
                }

            }
        }
        public string EffectiveDate { get => Patient.EffectiveDate;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.EffectiveDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public string FaxNo { get => Patient.FaxNo; set {
                bool valid = true;
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    Patient.FaxNo = value;
                }
                OnPropertyChanged(); } }
        public string ParentGuardianName { get => Patient.ParentGuardianName;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.ParentGuardianName = value;
                    OnPropertyChanged();
                }

            }
        }
        public string CellNo { get => Patient.CellNo; set {
                bool valid = true;
                CellNoError = "";
                if (String.IsNullOrEmpty(value))
                {
                    valid = false;
                    Patient.CellNo = "";
                    CellNoError = "Cell./Mobile No. is required.";
                }
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    if (value.Length < 12)
                    {
                        Patient.CellNo = value;
                    }
                    if (value.Length < 11)
                    {
                        CellNoError = "Must be an 11-digit number.";
                    }
                }
                OnPropertyChanged(); } }
        public string EmailAddress { get => Patient.EmailAddress;
            set
            {
                if (!value.Contains(" "))
                {
                    Patient.EmailAddress = value;
                    OnPropertyChanged();
                }

            }
        }
        public string Referral { get => Patient.Referral;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Referral = value;
                    OnPropertyChanged();
                }

            }
        }
        public string Reason { get => Patient.Reason;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Reason = value;
                    OnPropertyChanged();
                }

            }
        }
        public string PreviousDentist { get => Patient.PreviousDentist;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.PreviousDentist = value;
                    OnPropertyChanged();
                }

            }
        }
        public string LastDentalVisit { get => Patient.LastDentalVisit;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.LastDentalVisit = value;
                    OnPropertyChanged();
                }

            }
        }
        public string PhysicianName { get => Patient.PhysicianName;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.PhysicianName = value;
                    if (value.Trim().Equals(""))
                    {
                        PhysycianSpecialty = "";
                        PhysicianOfficeAddress = "";
                        PhysicianOfficeNumber = "";
                    }
                    OnPropertyChanged();
                }

            }
        }
        public string PhysycianSpecialty { get => Patient.PhysicianSpecialty;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.PhysicianSpecialty = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PhysicianOfficeAddress { get => Patient.PhysicianOfficeAddress;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.PhysicianOfficeAddress = value;
                    OnPropertyChanged();
                }

            }
        }
        public string PhysicianOfficeNumber { get => Patient.PhysicianOfficeNumber; set {
                bool valid = true;
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    Patient.PhysicianOfficeNumber = value;
                }
                OnPropertyChanged(); } }
        public bool IsInGoodHealth { get => Patient.IsInGoodHealth; set { Patient.IsInGoodHealth = value; OnPropertyChanged(); } }
        public string ConditionBeingTreated { get => Patient.ConditionBeingTreated;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.ConditionBeingTreated = value;
                    OnPropertyChanged();
                }

            }
        }
        public string IllnessOrOperation { get => Patient.IllnessOrOperation;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.IllnessOrOperation = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Hospitalization { get => Patient.Hospitalization;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Hospitalization = value;
                    OnPropertyChanged();
                }
            }
        }
        public string MedicationTaken { get => Patient.MedicationTaken;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.MedicationTaken = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsTobaccoUser { get => Patient.IsTobaccoUser; set { Patient.IsTobaccoUser = value; OnPropertyChanged(); } }
        public bool IsAlcoholCocaineDangerousDrugUser { get => Patient.IsAlcoholCocaineDangerousDrugUser; set { Patient.IsAlcoholCocaineDangerousDrugUser = value; OnPropertyChanged(); } }
        public string Allergies { get => Patient.Allergies;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Allergies = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BleedingTime { get => Patient.BleedingTime;
            set
            {
                bool valid = true;
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    Patient.BleedingTime = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsPregnant { get => Patient.IsPregnant; set { Patient.IsPregnant = value; OnPropertyChanged(); } }
        public bool IsNursing { get => Patient.IsNursing; set { Patient.IsNursing = value; OnPropertyChanged(); } }
        public bool IsTakingBirthControlPills { get => Patient.IsTakingBirthControlPills; set { Patient.IsTakingBirthControlPills = value; OnPropertyChanged(); } }
        public string BloodType { get => Patient.BloodType; set { Patient.BloodType = value; OnPropertyChanged(); } }
        public string BloodPressure { get => Patient.BloodPressure;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.BloodPressure = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Diseases { get => Patient.Diseases;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Patient.Diseases = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Image { get => Patient.Image; set { Patient.Image = value; OnPropertyChanged(); } }

        public List<string> Genders { get => genders; set => genders = value; }
        public Patient CopyPatient { get => copyPatient; set { copyPatient = value; OnPropertyChanged(); } }

        public string FirstNameError { get => firstNameError; set { firstNameError = value; OnPropertyChanged(); } }
        public string LastNameError { get => lastNameError; set { lastNameError = value; OnPropertyChanged(); } }
        public string CellNoError { get => cellNoError; set { cellNoError = value; OnPropertyChanged(); } }
        public string HomeAddressError { get => homeAddressError; set { homeAddressError = value; OnPropertyChanged(); } }

        private string firstNameError = "";
        private string lastNameError = "";
        private string cellNoError = "";
        private string homeAddressError = "";
    }
}
