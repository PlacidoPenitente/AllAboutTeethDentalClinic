﻿using System;
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
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                info.SetValue(this, info.GetValue(this));
            }
            bool hasError = false;
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                if (info.Name.EndsWith("Error"))
                {
                    if (!((string)info.GetValue(this)).Equals(""))
                    {
                        hasError = true;
                        break;
                    }
                }
            }
            if (!hasError)
            {
                Patient.AddedBy = ActiveUser;
                startUpdateToDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        public override void resetForm()
        {
            Patient = (Patient)CopyPatient.Clone();
        }
    }
}
