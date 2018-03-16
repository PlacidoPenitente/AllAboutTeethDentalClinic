using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Treatments
{
    public class EditTreatmentViewModel : AddTreatmentViewModel
    {
        public override void saveTreatment()
        {
            Treatment.AddedBy = ActiveUser;
            updateDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public override void resetForm()
        {
            Treatment = (Treatment)CopyTreatment.Clone();
        }
    }
}
