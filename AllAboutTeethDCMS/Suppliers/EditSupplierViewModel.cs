using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Suppliers
{
    public class EditSupplierViewModel : AddSupplierViewModel
    {
        public override void saveSupplier()
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
                    else
                    {
                        DialogBoxViewModel.Mode = "Error";
                        DialogBoxViewModel.Title = "Save Failed";
                        DialogBoxViewModel.Message = "Form contains errors. Please check all required fields.";
                        DialogBoxViewModel.Answer = "None";
                    }
                }
            }
            if (!hasError)
            {
                startUpdateToDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        public override void startResetThread()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Confirm";
            DialogBoxViewModel.Title = "Reset Form";
            DialogBoxViewModel.Message = "Resetting form will restore previous values. Proceed?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("OK"))
            {
                Supplier = (Supplier)CopySupplier.Clone();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    if (info.Name.EndsWith("Error"))
                    {
                        info.SetValue(this, "");
                    }
                }
            }
            DialogBoxViewModel.Answer = "";
        }
    }
}
