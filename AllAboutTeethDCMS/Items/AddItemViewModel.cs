using AllAboutTeethDCMS.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Items
{
    public class AddItemViewModel : CRUDPage<Item>
    {
        private Item item;
        private Item copyItem;
        private string error = "";
        private List<Supplier> suppliers;
        private int count = 0;

        public AddItemViewModel()
        {
            SupplierViewModel supplierViewModel = new SupplierViewModel();
            supplierViewModel.loadSuppliers();
            Suppliers = supplierViewModel.Suppliers;
            item = new Item();
        }

        public virtual void resetForm()
        {
            Item = new Item();
        }

        public virtual void saveSupplier()
        {
            Item.AddedBy = ActiveUser;
            for (int i = 0; i < count; i++)
            {
                saveToDatabase(Item, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        protected override void setLoaded(List<Item> list)
        {
            throw new NotImplementedException();
        }

        public Item Item
        {
            get => item;
            set
            {
                item = value;
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }

        public string Name { get => Item.Name; set { Item.Name = value; OnPropertyChanged(); } }
        public string Description { get => Item.Description; set { Item.Description = value; OnPropertyChanged(); } }
        public Supplier Supplier { get => Item.Supplier; set { Item.Supplier = value; OnPropertyChanged(); } }

        public Item CopyItem { get => copyItem; set { copyItem = value; OnPropertyChanged(); } }
        public string Error { get => error; set { error = value; OnPropertyChanged(); } }

        public List<Supplier> Suppliers { get => suppliers; set { suppliers = value; OnPropertyChanged(); } }
        public string Count { get => count.ToString(); set {
                try
                {
                    int num = Int32.Parse(value);
                    count = num;
                }
                catch(Exception ex)
                {
                    count = 0;
                }
                if(count<0)
                {
                    count = 0;
                }
                OnPropertyChanged(); } }
    }
}
