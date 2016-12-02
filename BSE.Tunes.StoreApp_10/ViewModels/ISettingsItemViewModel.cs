using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public interface ISettingsItemViewModel
    {
        string Title
        {
            get;set;
        }
        string Icon
        {
            get;set;
        }
        string Description
        {
            get;set;
        }
        ICommand NavigationCommand
        {
            get;
        }
    }
}
