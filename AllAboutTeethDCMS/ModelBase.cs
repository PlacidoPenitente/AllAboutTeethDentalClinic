using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS
{
    public class ModelBase : ICloneable
    {
        public object Clone()
        {
            var clone = Activator.CreateInstance(GetType());
            PropertyInfo[] propertyInfos = clone.GetType().GetProperties();
            for (int i = 0; i < GetType().GetProperties().Count(); i++)
            {
                propertyInfos[i].SetValue(clone, GetType().GetProperties().ElementAt(i).GetValue(this));
            }
            return clone;
        }

        public string validate([CallerMemberName] String propertyName = null)
        {
            string error = "";
            return error;
        }
    }
}
