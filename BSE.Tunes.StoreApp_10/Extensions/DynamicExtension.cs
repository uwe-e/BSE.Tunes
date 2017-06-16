using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Extensions
{
    public static class DynamicExtension
    {
        /// <summary>
        /// Returns an object that represents the specified public event declared by the current type
        /// </summary>
        /// <param name="dynamicObject">The dynamic type casted into an object</param>
        /// <param name="name">The name of the event.</param>
        /// <returns>An object that represents the specified event, if found; otherwise, null.</returns>
        /// <remarks>
        /// The release configuration uses .NET Native to compile the app to native code. This extension prevents runtime exceptions.
        /// </remarks>
        public static EventInfo GetDeclaredEvent(this object dynamicObject, string name)
        {
            EventInfo eventInfo = null;
            if (dynamicObject != null)
            {
                Type type = dynamicObject.GetType();
                eventInfo = type.GetTypeInfo().GetDeclaredEvent(name);
            }
            return eventInfo;
        }
    }
}
