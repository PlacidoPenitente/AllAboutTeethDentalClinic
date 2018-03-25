using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Suppliers
{
    public class SupplierViewModel : CRUDPage<Supplier>
    {
        private Supplier supplier;
        private List<Supplier> suppliers;
        private string filter = "";

        public Supplier Supplier { get => supplier; set { supplier = value; OnPropertyChanged(); } }
        public List<Supplier> Suppliers { get => suppliers; set { suppliers = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); loadSuppliers(); } }

        public void loadSuppliers()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteTreatment()
        {
            startDeleteFromDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Supplier> list)
        {
            Suppliers = list;
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
