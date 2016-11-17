using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Models
{
    public class User :  BSE.Tunes.Data.TunesUser
    {
        public bool UseSecureLogin
        {
            get;
            set;
        }
    }
}
