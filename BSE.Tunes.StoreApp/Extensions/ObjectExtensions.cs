using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE.Tunes.StoreApp.Attributes;

namespace BSE.Tunes.StoreApp.Extensions
{
    public static class ObjectExtensions
    {
        public static T GetCustomAttribute<T>(this object item) where T : Attribute
        {
            T attribute = default(T);
            if (item != null)
            {
                System.Reflection.MemberInfo memberInfo = item.GetType().GetTypeInfo();
                if (memberInfo != null)
                {
                    attribute = (T)memberInfo.GetCustomAttribute(typeof(T));
                }
            }
            return attribute;
        }
    }
}
