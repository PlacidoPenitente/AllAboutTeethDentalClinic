using AllAboutTeethDCMS.TreatmentRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.TreatmentRecords
{
    public class TreatmentRecordViewModel : CRUDPage<TreatmentRecord>
    {
        private TreatmentRecord treatmentRecord;
        private List<TreatmentRecord> treatmentRecords;
        private string filter = "";

        public TreatmentRecord TreatmentRecord { get => treatmentRecord; set { treatmentRecord = value; OnPropertyChanged(); } }
        public List<TreatmentRecord> TreatmentRecords { get => treatmentRecords; set { treatmentRecords = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadTreatmentRecords(); OnPropertyChanged(); } }

        public void loadTreatmentRecords()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteTreatmentRecord()
        {
            startDeleteFromDatabase(TreatmentRecord, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<TreatmentRecord> list)
        {
            TreatmentRecords = list;
        }
    }
}
