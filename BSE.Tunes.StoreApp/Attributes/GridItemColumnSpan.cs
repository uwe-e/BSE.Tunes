using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Attributes
{
    [AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Struct)]
    public class GridItemColumnSpan : Attribute
    {
        #region Properties
        public int ColumnSpan { get; private set; }
        #endregion

        #region MethodsPublic
        public GridItemColumnSpan(int columnSpan)
        {
            this.ColumnSpan = columnSpan;
        }
        #endregion
    }
}
