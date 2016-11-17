using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public interface IDialogService
    {
        Task ShowAsync(string content, string title = default(string));
    }
}
