using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.Entities
{
    public partial class TunesEntities : DbContext
    {
        public TunesEntities(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }
        public System.Data.Objects.ObjectContext ObjectContext()
        {
            return (this as System.Data.Entity.Infrastructure.IObjectContextAdapter).ObjectContext;
        }
    }
}
