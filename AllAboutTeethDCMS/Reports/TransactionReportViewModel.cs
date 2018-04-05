using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Reports
{
    public class TransactionReportViewModel : CRUDPage<TransactionReport>
    {
        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<TransactionReport> list)
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
