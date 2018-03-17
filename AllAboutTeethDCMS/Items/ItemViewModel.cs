using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Items
{
    public class ItemViewModel : CRUDPage<Item>
    {
        private Item item;
        private List<Item> items;
        private string filter = "";

        public Item Item { get => item; set { item = value; OnPropertyChanged(); } }
        public List<Item> Items { get => items; set { items = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); } }

        public void loadPatients()
        {
            Items = loadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteTreatment()
        {
            deleteFromDatabase(Item, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            loadPatients();
        }

        protected override void setLoaded(List<Item> list)
        {
            throw new NotImplementedException();
        }
    }
}
