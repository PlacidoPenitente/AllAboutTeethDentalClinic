using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Treatments;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.TreatmentRecords
{
    public class AddTreatmentRecordViewModel : CRUDPage<TreatmentRecord>
    {
        private TreatmentRecord treatmentRecord;

        public AddTreatmentRecordViewModel()
        {
            treatmentRecord = new TreatmentRecord();
        }

        public Patient Patient { get => TreatmentRecord.Patient; set { TreatmentRecord.Patient = value; OnPropertyChanged(); } }
        public Appointment Appointment { get => TreatmentRecord.Appointment; set { TreatmentRecord.Appointment = value; OnPropertyChanged(); } }
        public Tooth Tooth { get => TreatmentRecord.Tooth; set { TreatmentRecord.Tooth = value; OnPropertyChanged(); } }
        public Treatment Treatment { get => TreatmentRecord.Treatment; set { TreatmentRecord.Treatment = value; OnPropertyChanged(); } }
        public string Notes { get => TreatmentRecord.Notes; set { TreatmentRecord.Notes = value; OnPropertyChanged(); } }

        public TreatmentRecord TreatmentRecord { get => treatmentRecord; set { treatmentRecord = value; OnPropertyChanged(); } }

        public void saveTreatmentRecord()
        {
            SaveToDatabase(TreatmentRecord, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override bool beforeCreate()
        {
            throw new NotImplementedException();
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<TreatmentRecord> list)
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
