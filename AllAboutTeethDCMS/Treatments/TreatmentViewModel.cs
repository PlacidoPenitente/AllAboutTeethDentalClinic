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
        public string Filter { get => filter; set { filter = value; loadTreatments(); OnPropertyChanged(); } }

        public void loadTreatments()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteTreatment()
        {
            startDeleteFromDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Treatment> list)
        {
            Treatments = list;
        }
    }
}
