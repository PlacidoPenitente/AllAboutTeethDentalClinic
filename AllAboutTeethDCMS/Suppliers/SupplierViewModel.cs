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
        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); } }

        public void loadPatients()
        {
            Suppliers = loadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteTreatment()
        {
            startDeleteFromDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Supplier> list)
        {
            throw new NotImplementedException();
        }
    }
}
