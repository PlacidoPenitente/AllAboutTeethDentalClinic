using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Medicines
{
    public class MedicineViewModel : CRUDPage<Medicine>
    {
        private Medicine item;
        private List<Medicine> items;
        private string filter = "";

        public Medicine Medicine { get => item; set { item = value; OnPropertyChanged(); } }
        public List<Medicine> Medicines { get => items; set { items = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadMedicines(); OnPropertyChanged(); } }

        public void loadMedicines()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteMedicine()
        {
            startDeleteFromDatabase(Medicine, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Medicine> list)
        {
            Medicines = list;
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
    }
}
