using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllAboutTeethDCMS.Appointments;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Billings
{
    public class AddBillingViewModel : CRUDPage<Billing>
    {
        private Billing billing;

        public AddBillingViewModel()
        {
            Billing = new Billing();
        }

        public Appointment Appointment { get => Billing.Appointment; set => Billing.Appointment = value; }
        public double AmountCharged { get => Billing.AmountCharged; set => Billing.AmountCharged = value; }
        public double Balance { get => Billing.Balance; set => Billing.Balance = value; }
        public Billing Billing { get => billing; set => billing = value; }

        public void saveBilling()
        {
            SaveToDatabase(Billing, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<Billing> list)
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
