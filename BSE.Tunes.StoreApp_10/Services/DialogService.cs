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
        public static DialogService Instance { get; } = new DialogService();
        public async Task ShowAsync(string content, string title = default(string))
        {
            //AlertDialog.Builder alert = new AlertDialog.Builder(Application.Context);
            var dialog = (title == default(string)) ? new MessageDialog(content) : new MessageDialog(content, title);
            await dialog.ShowAsync();
        }
    }
}
