using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Providers
{
    public class ProviderViewModel : CRUDPage<Provider>
    {
        private Provider provider;
        private List<Provider> providers;
        private string filter = "";

        public Provider Provider { get => provider; set { provider = value; OnPropertyChanged(); } }
        public List<Provider> Providers { get => providers; set { providers = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadProviders(); OnPropertyChanged(); } }

        public void loadProviders()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteProvider()
        {
            startDeleteFromDatabase(Provider, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Provider> list)
        {
            Providers = list;
        }
    }
}
