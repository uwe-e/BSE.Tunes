using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices.WindowsRuntime;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238


/*
 Step 6: Install the Windows Service

In this step, you install the Windows service and run it from the Services console.

    Rebuild the solution and open a Visual Studio command prompt.
    Browse to the bin directory of the project where WindowsService1.exe is located.
    Run the following command to install the service:


    Installutil WindowsService1.exe 

    Start your service. To do so, click Start, click Run, type services.msc and then click OK. Right-click your service and then click Start.

 
 */

namespace BSE.Tunes.StoreApp.Views
{
    /// <summary>
	/// An page that displays a collection of covers of the available albums. The page supports incremental loading
    /// </summary>
    public sealed partial class AlbumsPage : BSE.Tunes.StoreApp.Common.LayoutAwarePage
    {
        public AlbumsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
    }
}
