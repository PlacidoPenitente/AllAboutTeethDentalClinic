using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Treatments
{
    public class AddTreatmentViewModel : CRUDPage<Treatment>
    {
        private Treatment treatment;
        private Treatment copyTreatment;

        public AddTreatmentViewModel()
        {
            treatment = new Treatment();
            copyTreatment = (Treatment)treatment.Clone();
        }

        public virtual void resetForm()
        {
            Treatment = new Treatment();
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
                Treatment.AddedBy = ActiveUser;
                startSaveToDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        protected override void setLoaded(List<Treatment> list)
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

        public string Name { get => Treatment.Name; set { Treatment.Name = value; NameError = ""; NameError = validateUniqueName(value, CopyTreatment.Name, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", "")); OnPropertyChanged(); } }
        public string Description { get => Treatment.Description; set { Treatment.Description = value; OnPropertyChanged(); } }

        public Treatment Treatment
        {
            get => treatment;
            set
            {
                treatment = value;
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }
        
        public Treatment CopyTreatment { get => copyTreatment; set { copyTreatment = value; } }

        public string NameError { get => nameError; set { nameError = value; OnPropertyChanged(); } }

        private string nameError = "";
    }
}
