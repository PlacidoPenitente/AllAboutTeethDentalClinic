using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Providers
{
    public class AddProviderViewModel : CRUDPage<Provider>
    {
        private Provider provider;
        private Provider copyProvider;

        public AddProviderViewModel()
        {
            provider = new Provider();
            copyProvider = (Provider)provider.Clone();
        }

        public virtual void resetForm()
        {
            Provider = new Provider();
        }

        public virtual void saveProvider()
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
                Provider.AddedBy = ActiveUser;
                startSaveToDatabase(Provider, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        protected override void setLoaded(List<Provider> list)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeSave()
        {
            throw new NotImplementedException();
        }

        protected override void afterSave(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        public string Name { get => Provider.Name; set { Provider.Name = value; NameError = ""; NameError = validateUniqueName(value, CopyProvider.Name, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", "")); OnPropertyChanged(); } }
        public string ContactNo { get => Provider.ContactNo; set { Provider.ContactNo = value; OnPropertyChanged(); } }
        public string Address { get => Provider.Address; set { Provider.Address = value; OnPropertyChanged(); } }

        public Provider Provider
        {
            get => provider;
            set
            {
                provider = value;
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }

        public Provider CopyProvider { get => copyProvider; set { copyProvider = value; } }

        public string NameError { get => nameError; set { nameError = value; OnPropertyChanged(); } }

        private string nameError = "";
    }
}
