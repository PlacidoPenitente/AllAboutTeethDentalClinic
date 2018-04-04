using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.Medicines;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.UsedItems
{
    public class AddUsedItemViewModel : CRUDPage<UsedItem>
    {
        private UsedItem usedItem;

        public AddUsedItemViewModel()
        {
            UsedItem = new UsedItem();
        }

        public Medicine Medicine { get => UsedItem.Medicine; set => UsedItem.Medicine = value; }
        public Appointment Appointment { get => UsedItem.Appointment; set => UsedItem.Appointment = value; }
        public int Quantity { get => UsedItem.Quantity; set => UsedItem.Quantity = value; }


        public void SaveUsedItem()
        {
            SaveToDatabase(UsedItem, "allaboutteeth_useditems");
        }

        public UsedItem UsedItem { get => usedItem; set => usedItem = value; }

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<UsedItem> list)
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeCreate()
        {
            throw new NotImplementedException();
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
