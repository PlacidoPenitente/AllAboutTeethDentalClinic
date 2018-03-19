﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AllAboutTeethDCMS.Providers;

namespace AllAboutTeethDCMS.Treatments
{
    public class EditTreatmentViewModel : AddTreatmentViewModel
    {

        public override void saveTreatment()
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
                Treatment.AddedBy = ActiveUser;
                updateDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        public override void resetForm()
        {
            Treatment = (Treatment)CopyTreatment.Clone();
        }
    }
}
