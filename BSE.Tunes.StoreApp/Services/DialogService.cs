using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace BSE.Tunes.StoreApp.Services
{
    public class DialogService : IDialogService
    {
        public async void ShowDialog(string content)
        {
            MessageDialog messageDialog = new MessageDialog(content);
            await messageDialog.ShowAsync();
        }
        public async void ShowDialog(string content, string title)
        {
            MessageDialog messageDialog = new MessageDialog(content, title);
            await messageDialog.ShowAsync();
        }
    }
}
