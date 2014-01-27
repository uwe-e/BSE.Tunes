using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public interface IResourceService
    {
        string GetString(string key);
        string GetString(string key, string defaultValue);
    }
}
