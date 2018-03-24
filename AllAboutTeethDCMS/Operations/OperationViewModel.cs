using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Operations;
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

        protected override void setLoaded(List<Operation> list)
        {
            Operations = list;
        }
    }
}
