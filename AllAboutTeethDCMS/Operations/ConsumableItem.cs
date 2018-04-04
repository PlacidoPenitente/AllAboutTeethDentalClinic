﻿using AllAboutTeethDCMS.Medicines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Operations
{
    public class ConsumableItem
    {
        private Medicine medicine;
        private MedicineViewModel medicineViewModel;
        private string consumed = "0";

        public Medicine Medicine { get => medicine; set => medicine = value; }
        public string Consumed { get => consumed;
            set
            {
                bool valid = true;
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    consumed = value;
                    if (String.IsNullOrEmpty(consumed))
                    {
                        consumed = "0";
                    }
                }
            }
        }
        public MedicineViewModel ViewModel { get => medicineViewModel; set => medicineViewModel = value; }

        public ConsumableItem()
        {
            ViewModel = new MedicineViewModel();
        }

        public void consume()
        {
            Medicine.Quantity = Medicine.Quantity - Int32.Parse(Consumed);
            ViewModel.UpdateDatabase(Medicine, "allaboutteeth_medicines");
        }
    }
}
