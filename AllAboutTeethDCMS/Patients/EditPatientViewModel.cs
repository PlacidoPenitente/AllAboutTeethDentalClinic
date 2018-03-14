using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Patients
{
    public class EditPatientViewModel : CRUDPage<Patient>
    {
        private Patient patient;
        private Patient copyPatient;
        private List<string> genders = new List<string>() { "Male", "Female" };

        public EditPatientViewModel()
        {
            patient = new Patient();
        }

        public void savePatient()
        {
            Patient.AddedBy = ActiveUser;
            updateDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public Patient Patient
        {
            get => patient;
            set
            {
                patient = value;
                CopyPatient = (Patient)value.Clone();
                Console.Write(value.FirstName);
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }
        public string LastName { get => Patient.LastName; set { Patient.LastName = value; OnPropertyChanged(); } }
        public string FirstName { get => Patient.FirstName; set { Patient.FirstName = value; OnPropertyChanged(); } }
        public string MiddleName { get => Patient.MiddleName; set { Patient.MiddleName = value; OnPropertyChanged(); } }
        public DateTime Birthdate
        {
            get => Patient.Birthdate; set
            {
                Patient.Birthdate = value;
                Occupation = "";
                DentalInsurance = "";
                ParentGuardianName = "";
                OnPropertyChanged();
            }
        }
        public string Sex { get => Patient.Sex; set { Patient.Sex = value; OnPropertyChanged(); } }
        public string Religion { get => Patient.Religion; set { Patient.Religion = value; OnPropertyChanged(); } }
        public string Nationality { get => Patient.Nationality; set { Patient.Nationality = value; OnPropertyChanged(); } }
        public string Nickname { get => Patient.Nickname; set { Patient.Nickname = value; OnPropertyChanged(); } }
        public string HomeAddress { get => Patient.HomeAddress; set { Patient.HomeAddress = value; OnPropertyChanged(); } }
        public string HomeNo { get => Patient.HomeNo; set { Patient.HomeNo = value; OnPropertyChanged(); } }
        public string Occupation
        {
            get => Patient.Occupation; set
            {
                Patient.Occupation = value;
                if (value.Trim().Equals(""))
                {
                    OfficeNo = "";
                    FaxNo = "";
                }
                OnPropertyChanged();
            }
        }
        public string OfficeNo { get => Patient.OfficeNo; set { Patient.OfficeNo = value; OnPropertyChanged(); } }
        public string DentalInsurance { get => Patient.DentalInsurance; set { Patient.DentalInsurance = value; OnPropertyChanged(); } }
        public string EffectiveDate { get => Patient.EffectiveDate; set { Patient.EffectiveDate = value; OnPropertyChanged(); } }
        public string FaxNo { get => Patient.FaxNo; set { Patient.FaxNo = value; OnPropertyChanged(); } }
        public string ParentGuardianName { get => Patient.ParentGuardianName; set => Patient.ParentGuardianName = value; }
        public string CellNo { get => Patient.CellNo; set { Patient.CellNo = value; OnPropertyChanged(); } }
        public string EmailAddress { get => Patient.EmailAddress; set { Patient.EmailAddress = value; OnPropertyChanged(); } }
        public string Referral { get => Patient.Referral; set { Patient.Referral = value; OnPropertyChanged(); } }
        public string Reason { get => Patient.Reason; set { Patient.Reason = value; OnPropertyChanged(); } }
        public string PreviousDentist { get => Patient.PreviousDentist; set { Patient.PreviousDentist = value; OnPropertyChanged(); } }
        public string LastDentalVisit { get => Patient.LastDentalVisit; set { Patient.LastDentalVisit = value; OnPropertyChanged(); } }
        public string PhysicianName
        {
            get => Patient.PhysicianName; set
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
        public string PhysycianSpecialty { get => Patient.PhysicianSpecialty; set { Patient.PhysicianSpecialty = value; OnPropertyChanged(); } }
        public string PhysicianOfficeAddress { get => Patient.PhysicianOfficeAddress; set { Patient.PhysicianOfficeAddress = value; OnPropertyChanged(); } }
        public string PhysicianOfficeNumber { get => Patient.PhysicianOfficeNumber; set { Patient.PhysicianOfficeNumber = value; OnPropertyChanged(); } }
        public bool IsInGoodHealth { get => Patient.IsInGoodHealth; set { Patient.IsInGoodHealth = value; OnPropertyChanged(); } }
        public string ConditionBeingTreated { get => Patient.ConditionBeingTreated; set { Patient.ConditionBeingTreated = value; OnPropertyChanged(); } }
        public string IllnessOrOperation { get => Patient.IllnessOrOperation; set { Patient.IllnessOrOperation = value; OnPropertyChanged(); } }
        public string Hospitalization { get => Patient.Hospitalization; set { Patient.Hospitalization = value; OnPropertyChanged(); } }
        public string MedicationTaken { get => Patient.MedicationTaken; set { Patient.MedicationTaken = value; OnPropertyChanged(); } }
        public bool IsTobaccoUser { get => Patient.IsTobaccoUser; set { Patient.IsTobaccoUser = value; OnPropertyChanged(); } }
        public bool IsAlcoholCocaineDangerousDrugUser { get => Patient.IsAlcoholCocaineDangerousDrugUser; set { Patient.IsAlcoholCocaineDangerousDrugUser = value; OnPropertyChanged(); } }
        public string Allergies { get => Patient.Allergies; set { Patient.Allergies = value; OnPropertyChanged(); } }
        public string BleedingTime { get => Patient.BleedingTime; set { Patient.BleedingTime = value; OnPropertyChanged(); } }
        public bool IsPregnant { get => Patient.IsPregnant; set { Patient.IsPregnant = value; OnPropertyChanged(); } }
        public bool IsNursing { get => Patient.IsNursing; set { Patient.IsNursing = value; OnPropertyChanged(); } }
        public bool IsTakingBirthControlPills { get => Patient.IsTakingBirthControlPills; set { Patient.IsTakingBirthControlPills = value; OnPropertyChanged(); } }
        public string BloodType { get => Patient.BloodType; set { Patient.BloodType = value; OnPropertyChanged(); } }
        public string BloodPressure { get => Patient.BloodPressure; set { Patient.BloodPressure = value; OnPropertyChanged(); } }
        public string Diseases { get => Patient.Diseases; set { Patient.Diseases = value; OnPropertyChanged(); } }
        public string Image { get => Patient.Image; set { Patient.Image = value; OnPropertyChanged(); } }

        public List<string> Genders { get => genders; set => genders = value; }
        public Patient CopyPatient { get => copyPatient; set => copyPatient = value; }

        public void resetForm()
        {
            Patient = CopyPatient;
        }
    }
}
