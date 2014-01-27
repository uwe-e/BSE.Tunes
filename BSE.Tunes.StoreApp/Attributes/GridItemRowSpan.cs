using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Attributes
{
    [AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Struct)]
    public class GridItemRowSpan : Attribute
    {
        #region Properties
        public int RowSpan { get; private set; }
        #endregion

        #region MethodsPublic
        public GridItemRowSpan(int rowSpan)
        {
            this.RowSpan = rowSpan;
        }
        #endregion
    }
}
