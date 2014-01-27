using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BSE.Tunes.StoreApp.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <returns></returns>
        public static async Task WaitForNonZeroSizeAsync(this FrameworkElement frameworkElement)
        {
            while (frameworkElement.ActualWidth == 0 && frameworkElement.ActualHeight == 0)
            {
                var taskCompletionSource = new TaskCompletionSource<Object>();
                SizeChangedEventHandler sizeChangedEventHandler = null;
                sizeChangedEventHandler = (s, e) =>
                {
                    frameworkElement.SizeChanged -= sizeChangedEventHandler;
                    taskCompletionSource.SetResult(e);
                };
                frameworkElement.SizeChanged += sizeChangedEventHandler;
                await taskCompletionSource.Task;
            }
        }
    }
}
