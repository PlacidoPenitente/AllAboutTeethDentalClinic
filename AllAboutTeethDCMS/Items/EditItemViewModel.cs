using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Items
{
    public class EditItemViewModel : AddItemViewModel
    {
        public override void saveSupplier()
        {
            Item.AddedBy = ActiveUser;
            updateDatabase(Item, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public override void resetForm()
        {
            Item = (Item)CopyItem.Clone();
        }
    }
}
