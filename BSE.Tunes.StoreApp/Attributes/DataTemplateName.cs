using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Attributes
{
    [AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Struct)]
    public class DataTemplateName : Attribute
    {
        #region Properties
        public string TemplateName { get; private set; }
        #endregion

        #region MethodsPublic
        public DataTemplateName(string templateName)
        {
            this.TemplateName = templateName;
        }
        #endregion
    }
}
