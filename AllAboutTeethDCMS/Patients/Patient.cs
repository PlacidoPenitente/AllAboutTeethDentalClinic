using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Patients
{
    public class Patient : ICloneable
    {
        private int no = -1;

        private string lastName = "";
        private string firstName = "";
        private string middleName = "";
        private DateTime birthdate = DateTime.Now;
        private string sex = "Male";
        private string religion = "";
        private string nationality = "";
        private string nickname = "";
        private string homeAddress = "";
        private string homeNo = "";
        private string occupation = "";
        private string officeNo = "";
        private string dentalInsurance = "";
        private string effectiveDate = "";
        private string faxNo = "";
        private string parentGuardianName = "";
        private string cellNo = "";
        private string emailAddress = "";
        private string referral = "";
        private string reason = "";

        private string previousDentist = "";
        private string lastDentalVisit = "";

        private string physicianName = "";
        private string physicianSpecialty = "";
        private string physicianOfficeAddress = "";
        private string physicianOfficeNumber = "";
        private bool isInGoodHealth = true;
        private string conditionBeingTreated = "";
        private string illnessOrOperation = "";
        private string hospitalization = "";
        private string medicationTaken = "";
        private bool isTobaccoUser = false;
        private bool isAlcoholCocaineDangerousDrugUser = false;
        private string allergies = "";
        private string bleedingTime = "";
        private bool isPregnant = false;
        private bool isNursing = false;
        private bool isTakingBirthControlPills = false;
        private string bloodType = "";
        private string bloodPressure = "";
        private string diseases = "";

        private string image = "";
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;

        public int No { get => no; set => no = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string MiddleName { get => middleName; set => middleName = value; }
        public DateTime Birthdate { get => birthdate; set => birthdate = value; }
        public string Sex { get => sex; set => sex = value; }
        public string Religion { get => religion; set => religion = value; }
        public string Nationality { get => nationality; set => nationality = value; }
        public string Nickname { get => nickname; set => nickname = value; }
        public string HomeAddress { get => homeAddress; set => homeAddress = value; }
        public string HomeNo { get => homeNo; set => homeNo = value; }
        public string Occupation { get => occupation; set => occupation = value; }
        public string OfficeNo { get => officeNo; set => officeNo = value; }
        public string DentalInsurance { get => dentalInsurance; set => dentalInsurance = value; }
        public string EffectiveDate { get => effectiveDate; set => effectiveDate = value; }
        public string FaxNo { get => faxNo; set => faxNo = value; }
        public string ParentGuardianName { get => parentGuardianName; set => parentGuardianName = value; }
        public string CellNo { get => cellNo; set => cellNo = value; }
        public string EmailAddress { get => emailAddress; set => emailAddress = value; }
        public string Referral { get => referral; set => referral = value; }
        public string Reason { get => reason; set => reason = value; }
        public string PreviousDentist { get => previousDentist; set => previousDentist = value; }
        public string LastDentalVisit { get => lastDentalVisit; set => lastDentalVisit = value; }
        public string PhysicianName { get => physicianName; set => physicianName = value; }
        public string PhysicianSpecialty { get => physicianSpecialty; set => physicianSpecialty = value; }
        public string PhysicianOfficeAddress { get => physicianOfficeAddress; set => physicianOfficeAddress = value; }
        public string PhysicianOfficeNumber { get => physicianOfficeNumber; set => physicianOfficeNumber = value; }
        public bool IsInGoodHealth { get => isInGoodHealth; set => isInGoodHealth = value; }
        public string ConditionBeingTreated { get => conditionBeingTreated; set => conditionBeingTreated = value; }
        public string IllnessOrOperation { get => illnessOrOperation; set => illnessOrOperation = value; }
        public string Hospitalization { get => hospitalization; set => hospitalization = value; }
        public string MedicationTaken { get => medicationTaken; set => medicationTaken = value; }
        public bool IsTobaccoUser { get => isTobaccoUser; set => isTobaccoUser = value; }
        public bool IsAlcoholCocaineDangerousDrugUser { get => isAlcoholCocaineDangerousDrugUser; set => isAlcoholCocaineDangerousDrugUser = value; }
        public string Allergies { get => allergies; set => allergies = value; }
        public string BleedingTime { get => bleedingTime; set => bleedingTime = value; }
        public bool IsPregnant { get => isPregnant; set => isPregnant = value; }
        public bool IsNursing { get => isNursing; set => isNursing = value; }
        public bool IsTakingBirthControlPills { get => isTakingBirthControlPills; set => isTakingBirthControlPills = value; }
        public string BloodType { get => bloodType; set => bloodType = value; }
        public string BloodPressure { get => bloodPressure; set => bloodPressure = value; }
        public string Diseases { get => diseases; set => diseases = value; }
        public string Image { get => image; set => image = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }

        public object Clone()
        {
            var clone = Activator.CreateInstance(GetType());
            PropertyInfo[] propertyInfos = clone.GetType().GetProperties();
            for (int i = 0; i < GetType().GetProperties().Count(); i++)
            {
                propertyInfos[i].SetValue(clone, GetType().GetProperties().ElementAt(i).GetValue(this));
            }
            return clone;
        }
    }
}
