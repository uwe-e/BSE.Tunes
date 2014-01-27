using BSE.Tunes.Data;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class GenresViewModel : ViewModelBase
    {
        #region FieldsPrivate
        #endregion

        #region Properties

        public Genre Genre
        {
            get;
            private set;
        }
        #endregion

        #region MethodsPublic
        public GenresViewModel(Genre genre)
        {
            this.Genre = genre;
        }
        #endregion
    }
}
