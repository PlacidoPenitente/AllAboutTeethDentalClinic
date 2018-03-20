using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.DentalChart
{
    public class DentalChartViewModel : CRUDPage<Tooth>
    {
        private Patient patient;

        private Tooth tooth18;
        private Tooth tooth17;
        private Tooth tooth16;
        private Tooth tooth15;
        private Tooth tooth14;
        private Tooth tooth13;
        private Tooth tooth12;
        private Tooth tooth11;
        private Tooth tooth21;
        private Tooth tooth22;
        private Tooth tooth23;
        private Tooth tooth24;
        private Tooth tooth25;
        private Tooth tooth26;
        private Tooth tooth27;
        private Tooth tooth28;

        private Tooth tooth48;
        private Tooth tooth47;
        private Tooth tooth46;
        private Tooth tooth45;
        private Tooth tooth44;
        private Tooth tooth43;
        private Tooth tooth42;
        private Tooth tooth41;
        private Tooth tooth31;
        private Tooth tooth32;
        private Tooth tooth33;
        private Tooth tooth34;
        private Tooth tooth35;
        private Tooth tooth36;
        private Tooth tooth37;
        private Tooth tooth38;

        private Tooth tooth55;
        private Tooth tooth54;
        private Tooth tooth53;
        private Tooth tooth52;
        private Tooth tooth51;
        private Tooth tooth61;
        private Tooth tooth62;
        private Tooth tooth63;
        private Tooth tooth64;
        private Tooth tooth65;
        
        private Tooth tooth85;
        private Tooth tooth84;
        private Tooth tooth83;
        private Tooth tooth82;
        private Tooth tooth81;
        private Tooth tooth71;
        private Tooth tooth72;
        private Tooth tooth73;
        private Tooth tooth74;
        private Tooth tooth75;

        public DentalChartViewModel()
        {
            foreach(PropertyInfo info in GetType().GetProperties())
            {
                if(info.PropertyType.Name.Equals("Tooth"))
                {
                    info.SetValue(this, new Tooth()
                    {
                        ToothNo = info.Name.Replace("Tooth", "")
                    });
                }
            }
        }
        
        private List<Tooth> teeth;

        public List<Tooth> Teeth { get => teeth; set { teeth = value; OnPropertyChanged(); } }

        public Tooth Tooth18 { get => tooth18; set { tooth18 = value; OnPropertyChanged(); } }
        public Tooth Tooth17 { get => tooth17; set { tooth17 = value; OnPropertyChanged(); } }
        public Tooth Tooth16 { get => tooth16; set { tooth16 = value; OnPropertyChanged(); } }
        public Tooth Tooth15 { get => tooth15; set { tooth15 = value; OnPropertyChanged(); } }
        public Tooth Tooth14 { get => tooth14; set { tooth14 = value; OnPropertyChanged(); } }
        public Tooth Tooth13 { get => tooth13; set { tooth13 = value; OnPropertyChanged(); } }
        public Tooth Tooth12 { get => tooth12; set { tooth12 = value; OnPropertyChanged(); } }
        public Tooth Tooth11 { get => tooth11; set { tooth11 = value; OnPropertyChanged(); } }
        public Tooth Tooth21 { get => tooth21; set { tooth21 = value; OnPropertyChanged(); } }
        public Tooth Tooth22 { get => tooth22; set { tooth22 = value; OnPropertyChanged(); } }
        public Tooth Tooth23 { get => tooth23; set { tooth23 = value; OnPropertyChanged(); } }
        public Tooth Tooth24 { get => tooth24; set { tooth24 = value; OnPropertyChanged(); } }
        public Tooth Tooth25 { get => tooth25; set { tooth25 = value; OnPropertyChanged(); } }
        public Tooth Tooth26 { get => tooth26; set { tooth26 = value; OnPropertyChanged(); } }
        public Tooth Tooth27 { get => tooth27; set { tooth27 = value; OnPropertyChanged(); } }
        public Tooth Tooth28 { get => tooth28; set { tooth28 = value; OnPropertyChanged(); } }
        public Tooth Tooth48 { get => tooth48; set { tooth48 = value; OnPropertyChanged(); } }
        public Tooth Tooth47 { get => tooth47; set { tooth47 = value; OnPropertyChanged(); } }
        public Tooth Tooth46 { get => tooth46; set { tooth46 = value; OnPropertyChanged(); } }
        public Tooth Tooth45 { get => tooth45; set { tooth45 = value; OnPropertyChanged(); } }
        public Tooth Tooth44 { get => tooth44; set { tooth44 = value; OnPropertyChanged(); } }
        public Tooth Tooth43 { get => tooth43; set { tooth43 = value; OnPropertyChanged(); } }
        public Tooth Tooth42 { get => tooth42; set { tooth42 = value; OnPropertyChanged(); } }
        public Tooth Tooth41 { get => tooth41; set { tooth41 = value; OnPropertyChanged(); } }
        public Tooth Tooth31 { get => tooth31; set { tooth31 = value; OnPropertyChanged(); } }
        public Tooth Tooth32 { get => tooth32; set { tooth32 = value; OnPropertyChanged(); } }
        public Tooth Tooth33 { get => tooth33; set { tooth33 = value; OnPropertyChanged(); } }
        public Tooth Tooth34 { get => tooth34; set { tooth34 = value; OnPropertyChanged(); } }
        public Tooth Tooth35 { get => tooth35; set { tooth35 = value; OnPropertyChanged(); } }
        public Tooth Tooth36 { get => tooth36; set { tooth36 = value; OnPropertyChanged(); } }
        public Tooth Tooth37 { get => tooth37; set { tooth37 = value; OnPropertyChanged(); } }
        public Tooth Tooth38 { get => tooth38; set { tooth38 = value; OnPropertyChanged(); } }
        public Tooth Tooth55 { get => tooth55; set { tooth55 = value; OnPropertyChanged(); } }
        public Tooth Tooth54 { get => tooth54; set { tooth54 = value; OnPropertyChanged(); } }
        public Tooth Tooth53 { get => tooth53; set { tooth53 = value; OnPropertyChanged(); } }
        public Tooth Tooth52 { get => tooth52; set { tooth52 = value; OnPropertyChanged(); } }
        public Tooth Tooth51 { get => tooth51; set { tooth51 = value; OnPropertyChanged(); } }
        public Tooth Tooth61 { get => tooth61; set { tooth61 = value; OnPropertyChanged(); } }
        public Tooth Tooth62 { get => tooth62; set { tooth62 = value; OnPropertyChanged(); } }
        public Tooth Tooth63 { get => tooth63; set { tooth63 = value; OnPropertyChanged(); } }
        public Tooth Tooth64 { get => tooth64; set { tooth64 = value; OnPropertyChanged(); } }
        public Tooth Tooth65 { get => tooth65; set { tooth65 = value; OnPropertyChanged(); } }
        public Tooth Tooth85 { get => tooth85; set { tooth85 = value; OnPropertyChanged(); } }
        public Tooth Tooth84 { get => tooth84; set { tooth84 = value; OnPropertyChanged(); } }
        public Tooth Tooth83 { get => tooth83; set { tooth83 = value; OnPropertyChanged(); } }
        public Tooth Tooth82 { get => tooth82; set { tooth82 = value; OnPropertyChanged(); } }
        public Tooth Tooth81 { get => tooth81; set { tooth81 = value; OnPropertyChanged(); } }
        public Tooth Tooth71 { get => tooth71; set { tooth71 = value; OnPropertyChanged(); } }
        public Tooth Tooth72 { get => tooth72; set { tooth72 = value; OnPropertyChanged(); } }
        public Tooth Tooth73 { get => tooth73; set { tooth73 = value; OnPropertyChanged(); } }
        public Tooth Tooth74 { get => tooth74; set { tooth74 = value; OnPropertyChanged(); } }
        public Tooth Tooth75 { get => tooth75; set { tooth75 = value; OnPropertyChanged(); } }

        public Patient Patient { get => patient; set { patient = value; OnPropertyChanged(); } }

        protected override void setLoaded(List<Tooth> list)
        {

        }
    }
}
