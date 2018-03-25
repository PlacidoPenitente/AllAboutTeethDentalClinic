﻿using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.DentalCharts
{
    public class ToothViewModel : CRUDPage<Tooth>
    {
        private Tooth tooth;
        private bool isSelected = false;
        private List<ToothViewModel> teeth;
        
        public ToothViewModel()
        {
            tooth = new Tooth();
        }

        public Patient Owner { get => Tooth.Owner; set { Tooth.Owner = value; OnPropertyChanged(); } }
        public string Condition { get => Tooth.Condition; set { Tooth.Condition = value; OnPropertyChanged(); } }
        public string ToothNo { get => Tooth.ToothNo; set { Tooth.ToothNo = value; OnPropertyChanged(); } }
        public Tooth Tooth { get => tooth; set { tooth = value; OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            } }

        public bool IsSelected { get => isSelected; set { isSelected = value; OnPropertyChanged(); } }

        public List<ToothViewModel> Teeth { get => teeth; set => teeth = value; }

        public void loadTooth()
        {
            CustomFilter = "tooth_owner='" + Owner.No + "' AND tooth_toothno='"+ToothNo+"'";
            Tooth tooth = null;
            while (tooth==null)
            {
                List<Tooth> teeth = loadFromDatabase("allaboutteeth_tooths", "");
                if(teeth.Count>0)
                {
                    tooth = teeth.ElementAt(0);
                }
                else
                {
                    saveToDatabase(Tooth, "allaboutteeth_tooths");
                }
            }
            Tooth = tooth;
        }

        public void saveTooth()
        {
            updateDatabase(Tooth, "allaboutteeth_tooths");
        }

        protected override void afterSave()
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate()
        {
            throw new NotImplementedException();
        }

        protected override bool beforeSave()
        {
            throw new NotImplementedException();
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void setLoaded(List<Tooth> list)
        {
            if(list.Count<1)
            {
                startSaveToDatabase(Tooth, "allaboutteeth_tooths");
            }
            else
            {
                Tooth = list.ElementAt(0);
            }
        }
    }
}
