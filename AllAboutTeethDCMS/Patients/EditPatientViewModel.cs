using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Patients
{
    public class EditPatientViewModel : AddPatientViewModel
    {
        public override void savePatient()
        {
            Patient.AddedBy = ActiveUser;
            updateDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public override void resetForm()
        {
            Patient = (Patient)CopyPatient.Clone();
        }
    }
}
