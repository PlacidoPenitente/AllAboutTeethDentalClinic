﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Medicines
{
    public class EditMedicineViewModel : AddMedicineViewModel
    {
        public override void saveMedicine()
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
                Medicine.AddedBy = ActiveUser;
                updateDatabase(Medicine, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        public override void resetForm()
        {
            Medicine = (Medicine)CopyMedicine.Clone();
        }
    }
}
