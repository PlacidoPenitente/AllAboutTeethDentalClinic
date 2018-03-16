using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Treatments
{
    public class TreatmentViewModel : CRUDPage<Treatment>
    {
        private Treatment treatment;
        private List<Treatment> treatments;
        private string filter = "";

        public Treatment Treatment { get => treatment; set { treatment = value; OnPropertyChanged(); } }
        public List<Treatment> Treatments { get => treatments; set { treatments = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); } }

        public void loadPatients()
        {
            Treatments = loadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteTreatment()
        {
            deleteFromDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            loadPatients();
        }
    }
}
