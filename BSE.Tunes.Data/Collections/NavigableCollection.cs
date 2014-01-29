using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BSE.Tunes.Data.Collections
{
    public class NavigableCollection<T> : ObservableCollection<T>
    {
        #region Properties
        public int Index { get; private set; }
        public bool CanMoveNext
        {
            get
            {
                return (this.Index < (base.Count - 1));
            }
        }
        public bool CanMovePrevious
        {
            get
            {
                return (this.Index > 0);
            }
        }
        #endregion

        #region MethodsPublic
        public NavigableCollection()
        {
            //this.Index = -1;
        }
        public bool MoveNext()
        {
            this.Index++;
            return ((this.Index >= 0) && (this.Index < base.Count));
        }
        public bool MovePrevious()
        {
            bool flag = false;
            if (!this.CanMovePrevious)
            {
                return flag;
            }
            this.Index--;
            return ((this.Index >= 0) && (this.Index < base.Count));
        }
        public T Current
        {
            get
            {
                if ((this.Index < 0) || (this.Index >= base.Count))
                {
                    throw new IndexOutOfRangeException();
                }
                return base[this.Index];
            }
        }
        public void Reset()
        {
            this.Index = -1;
        }
        #endregion
    }
}
