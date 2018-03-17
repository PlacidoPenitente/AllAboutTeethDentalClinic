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
        private string error = "";
        private Treatment copyTreatment;

        public AddTreatmentViewModel()
        {
            treatment = new Treatment();
        }

        public virtual void resetForm()
        {
            Treatment = new Treatment();
        }

        public virtual void saveTreatment()
        {
            Treatment.AddedBy = ActiveUser;
            saveToDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Treatment> list)
        {
            throw new NotImplementedException();
        }

        public string Name { get => Treatment.Name; set => Treatment.Name = value; }
        public string Description { get => Treatment.Description; set => Treatment.Description = value; }

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
        public string Error { get => error; set { error = value; OnPropertyChanged(); } }
        
        public Treatment CopyTreatment { get => copyTreatment; set { copyTreatment = value; } }
    }
}
