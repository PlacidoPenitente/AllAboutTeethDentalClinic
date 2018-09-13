using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.TreatmentRecords;
using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.DentalCharts
{
    public class ToothViewModel : CRUDPage<Tooth>
    {
        private Tooth tooth;
        private bool isSelected = false;
        private bool isAllowed = true;

        private List<ToothViewModel> teeth;
        
        public ToothViewModel()
        {
            tooth = new Tooth();
        }

        public Patient Owner { get => Tooth.Owner; set { Tooth.Owner = value; OnPropertyChanged(); } }
        public string Condition { get => Tooth.Condition; set { Tooth.Condition = value; OnPropertyChanged(); } }
        public string ToothNo { get => Tooth.ToothNo; set { Tooth.ToothNo = value; OnPropertyChanged(); } }
        public string Remarks { get => Tooth.Remarks; set { Tooth.Remarks = value; OnPropertyChanged(); } }

        public TreatmentRecordViewModel TreatmentRecordViewModel { get; set; }

        internal void Reset()
        {
            //throw new NotImplementedException();
        }

        public void Load()
        {
            if (TreatmentRecordViewModel == null) return;
            TreatmentRecordViewModel.PatientNo = Owner.No;
            TreatmentRecordViewModel.ToothNo = ToothNo;
            TreatmentRecordViewModel.LoadTreatmentRecords();
        }

        public Tooth Tooth { get => tooth; set { tooth = value; OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            } }

        public bool IsSelected { get => isSelected; set { isSelected = value; OnPropertyChanged(); } }

        public List<ToothViewModel> Teeth { get => teeth; set => teeth = value; }
        public bool IsAllowed { get => isAllowed; set { isAllowed = value; OnPropertyChanged(); } }

        public void loadTooth()
        {
            //CustomFilter = "tooth_owner='" + Owner.No + "' AND tooth_toothno='"+ToothNo+"'";
            Tooth tooth = null;
            while (tooth==null)
            {
                List<Tooth> teeth = LoadFromDatabase("allaboutteeth_tooths", "");
                if(teeth.Count>0)
                {
                    tooth = teeth.ElementAt(0);
                }
                else
                {
                    SaveToDatabase(Tooth, "allaboutteeth_tooths");
                }
                teeth = null;
                GC.Collect();
            }
            Tooth = tooth;
        }

        public void saveTooth()
        {
            UpdateDatabase(Tooth, "allaboutteeth_tooths");
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

        protected override void afterLoad(List<Tooth> list)
        {
            if(list.Count<1)
            {
                startSaveToDatabase(Tooth, "allaboutteeth_tooths");
            }
            else
            {
                Tooth = list.ElementAt(0);
            }
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            if(Owner!=null)
            {
                command.Parameters.Clear();
                command.CommandText = "select * from allaboutteeth_tooths where tooth_owner='" + Owner.No + "' AND tooth_toothno='" + ToothNo + "'";
            }
        }
    }
}
