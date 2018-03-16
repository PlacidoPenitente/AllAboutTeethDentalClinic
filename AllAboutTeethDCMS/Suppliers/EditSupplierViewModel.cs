using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Suppliers
{
    public class EditSupplierViewModel : AddSupplierViewModel
    {
        public override void saveSupplier()
        {
            Supplier.AddedBy = ActiveUser;
            updateDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public override void resetForm()
        {
            Supplier = (Supplier)CopySupplier.Clone();
        }
    }
}
