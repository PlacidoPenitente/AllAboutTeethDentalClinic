using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Operations;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Operations
{
    public class OperationViewModel : CRUDPage<Operation>
    {
        private Operation operation;
        private List<Operation> operations;
        private string filter = "";

        public Operation Operation { get => operation; set { operation = value; OnPropertyChanged(); } }
        public List<Operation> Operations { get => operations; set { operations = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadOperations(); OnPropertyChanged(); } }

        public void loadOperations()
        {
        }

        public void deleteOperation()
        {
            startDeleteFromDatabase(Operation, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void afterLoad(List<Operation> list)
        {
            Operations = list;
        }

        protected override bool beforeUpdate()
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

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
