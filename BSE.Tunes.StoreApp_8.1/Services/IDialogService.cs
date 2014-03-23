using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public interface IDialogService
    {
        void ShowDialog(string content);
        void ShowDialog(string content, string title);
    }
}
