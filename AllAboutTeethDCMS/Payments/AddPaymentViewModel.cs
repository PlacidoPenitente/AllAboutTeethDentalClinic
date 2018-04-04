using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllAboutTeethDCMS.Billings;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Payments
{
    public class AddPaymentViewModel : CRUDPage<Payment>
    {
        private Payment payment;

        public AddPaymentViewModel()
        {
            Payment = new Payment();
        }

        public Billing Billing { get => Payment.Billing; set => Payment.Billing = value; }
        public double AmountPaid { get => Payment.AmountPaid; set => Payment.AmountPaid = value; }
        public double Balance { get => Payment.Balance; set => Payment.Balance = value; }
        public Payment Payment { get => payment; set => payment = value; }

        public void savePayment()
        {
            SaveToDatabase(Payment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<Payment> list)
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
