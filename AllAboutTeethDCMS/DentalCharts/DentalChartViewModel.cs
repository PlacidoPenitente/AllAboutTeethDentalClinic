using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.DentalChart
{
    public class DentalChartViewModel : ViewModelBase
    {
        private Patient patient;
        private User user;
        private List<ToothViewModel> teethView;
        private Thread loadTeethThread;
        private Thread updateTeethThread;

        private ToothViewModel toothView18;
        private ToothViewModel toothView17;
        private ToothViewModel toothView16;
        private ToothViewModel toothView15;
        private ToothViewModel toothView14;
        private ToothViewModel toothView13;
        private ToothViewModel toothView12;
        private ToothViewModel toothView11;
        private ToothViewModel toothView21;
        private ToothViewModel toothView22;
        private ToothViewModel toothView23;
        private ToothViewModel toothView24;
        private ToothViewModel toothView25;
        private ToothViewModel toothView26;
        private ToothViewModel toothView27;
        private ToothViewModel toothView28;

        private ToothViewModel toothView48;
        private ToothViewModel toothView47;
        private ToothViewModel toothView46;
        private ToothViewModel toothView45;
        private ToothViewModel toothView44;
        private ToothViewModel toothView43;
        private ToothViewModel toothView42;
        private ToothViewModel toothView41;
        private ToothViewModel toothView31;
        private ToothViewModel toothView32;
        private ToothViewModel toothView33;
        private ToothViewModel toothView34;
        private ToothViewModel toothView35;
        private ToothViewModel toothView36;
        private ToothViewModel toothView37;
        private ToothViewModel toothView38;

        private ToothViewModel toothView55;
        private ToothViewModel toothView54;
        private ToothViewModel toothView53;
        private ToothViewModel toothView52;
        private ToothViewModel toothView51;
        private ToothViewModel toothView61;
        private ToothViewModel toothView62;
        private ToothViewModel toothView63;
        private ToothViewModel toothView64;
        private ToothViewModel toothView65;

        private ToothViewModel toothView85;
        private ToothViewModel toothView84;
        private ToothViewModel toothView83;
        private ToothViewModel toothView82;
        private ToothViewModel toothView81;
        private ToothViewModel toothView71;
        private ToothViewModel toothView72;
        private ToothViewModel toothView73;
        private ToothViewModel toothView74;
        private ToothViewModel toothView75;

        public ToothViewModel ToothView18 { get => toothView18; set { toothView18 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView17 { get => toothView17; set { toothView17 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView16 { get => toothView16; set { toothView16 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView15 { get => toothView15; set { toothView15 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView14 { get => toothView14; set { toothView14 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView13 { get => toothView13; set { toothView13 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView12 { get => toothView12; set { toothView12 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView11 { get => toothView11; set { toothView11 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView21 { get => toothView21; set { toothView21 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView22 { get => toothView22; set { toothView22 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView23 { get => toothView23; set { toothView23 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView24 { get => toothView24; set { toothView24 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView25 { get => toothView25; set { toothView25 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView26 { get => toothView26; set { toothView26 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView27 { get => toothView27; set { toothView27 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView28 { get => toothView28; set { toothView28 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView48 { get => toothView48; set { toothView48 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView47 { get => toothView47; set { toothView47 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView46 { get => toothView46; set { toothView46 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView45 { get => toothView45; set { toothView45 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView44 { get => toothView44; set { toothView44 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView43 { get => toothView43; set { toothView43 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView42 { get => toothView42; set { toothView42 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView41 { get => toothView41; set { toothView41 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView31 { get => toothView31; set { toothView31 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView32 { get => toothView32; set { toothView32 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView33 { get => toothView33; set { toothView33 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView34 { get => toothView34; set { toothView34 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView35 { get => toothView35; set { toothView35 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView36 { get => toothView36; set { toothView36 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView37 { get => toothView37; set { toothView37 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView38 { get => toothView38; set { toothView38 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView55 { get => toothView55; set { toothView55 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView54 { get => toothView54; set { toothView54 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView53 { get => toothView53; set { toothView53 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView52 { get => toothView52; set { toothView52 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView51 { get => toothView51; set { toothView51 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView61 { get => toothView61; set { toothView61 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView62 { get => toothView62; set { toothView62 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView63 { get => toothView63; set { toothView63 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView64 { get => toothView64; set { toothView64 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView65 { get => toothView65; set { toothView65 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView85 { get => toothView85; set { toothView85 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView84 { get => toothView84; set { toothView84 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView83 { get => toothView83; set { toothView83 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView82 { get => toothView82; set { toothView82 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView81 { get => toothView81; set { toothView81 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView71 { get => toothView71; set { toothView71 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView72 { get => toothView72; set { toothView72 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView73 { get => toothView73; set { toothView73 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView74 { get => toothView74; set { toothView74 = value; OnPropertyChanged(); } }
        public ToothViewModel ToothView75 { get => toothView75; set { toothView75 = value; OnPropertyChanged(); } }

        public Patient Patient { get => patient;
            set
            {
                patient = value;
                TeethView = new List<ToothViewModel>();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    if (info.Name.StartsWith("ToothView"))
                    {
                        ToothViewModel toothView = (ToothViewModel)info.GetValue(this);
                        toothView.Teeth = TeethView;
                        toothView.Owner = value;
                        toothView.ActiveUser = User;
                    }
                    OnPropertyChanged(info.Name);
                }
                startLoadingTeeth();
                OnPropertyChanged();
            }
        }

        public void startLoadingTeeth()
        {
            if(loadTeethThread==null||!loadTeethThread.IsAlive)
            {
                loadTeethThread = new Thread(loadTeeth);
                loadTeethThread.IsBackground = true;
                loadTeethThread.Start();
            }
        }

        public void loadTeeth()
        {
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                if (info.Name.StartsWith("ToothView"))
                {
                    ToothViewModel toothView = (ToothViewModel)info.GetValue(this);
                    toothView.loadTooth();
                }
                OnPropertyChanged(info.Name);
            }
        }

        public void startUpdatingTeeth()
        {
            if (updateTeethThread == null || !updateTeethThread.IsAlive)
            {
                updateTeethThread = new Thread(updateTeeth);
                updateTeethThread.IsBackground = true;
                updateTeethThread.Start();
            }
        }

        private void updateTeeth()
        {
            foreach(ToothViewModel toothView in TeethView)
            {
                toothView.ActiveUser = User;
                toothView.saveTooth();
            }
        }

        public User User { get => user; set => user = value; }
        public List<ToothViewModel> TeethView { get => teethView; set => teethView = value; }
        public Thread LoadTeethThread { get => loadTeethThread; set { loadTeethThread = value; } }

        public DentalChartViewModel()
        {
            foreach(PropertyInfo info in GetType().GetProperties())
            {
                if(info.Name.StartsWith("ToothView"))
                {
                    info.SetValue(this, new ToothViewModel() {
                        Condition = "Present Teeth",
                        ToothNo = info.Name.Replace("ToothView","")
                    });
                }
            }
            teethView = new List<ToothViewModel>();
        }
    }
}
