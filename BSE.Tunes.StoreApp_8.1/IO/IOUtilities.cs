using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.IO
{
    public static class IOUtilities
    {
        	/// <summary>
/// Wraps sharing violations that could occur on a file IO operation.
/// </summary>
/// <param name="action">The action to execute. May not be null.</param>
/// <param name="exceptionsCallback">The exceptions callback. May be null.</param>
/// <param name="retryCount">The retry count.</param>
/// <param name="waitTime">The wait time in milliseconds.</param>
        public static void WrapSharingViolations(WrapSharingViolationsCallback action, WrapSharingViolationsExceptionsCallback exceptionsCallback = null, int retryCount = 10, int waitTime = 20)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (AggregateException ae)
                {
                    var sharingViolation = ae.Flatten().InnerExceptions.Select(ex => ex.IsSharingViolation());
                    if (ae.Flatten().InnerExceptions.Select(ex => ex.IsSharingViolation()).Any() && i < retryCount - 1)
                    {
                        //var wait = true;
                        //if (exceptionsCallback != null)
                        //{
                        //    wait = exceptionsCallback((current, i, retryCount, waitTime);
                        //}

                        //if ((IsSharingViolation(ioe) && (i < (retryCount - 1))) || (ioe is FileNotFoundException))
                        //{
                        //    var wait = true;
                        //    if (exceptionsCallback != null)
                        //    {
                        //        wait = exceptionsCallback(ioe, i, retryCount, waitTime);
                        //    }
                        //    if (wait)
                        //    {
                        //        System.Threading.Thread.Sleep(waitTime);
                        //    }
                        //}
                        //else
                        //{
                        //    throw;
                        //}
                    }
                }
            }
        }
        /// <summary>
        /// Defines a sharing violation wrapper delegate.
        /// </summary>
        public delegate void WrapSharingViolationsCallback();
        /// <summary>
        /// Defines a sharing violation wrapper delegate for handling exception.
        /// </summary>
        public delegate bool WrapSharingViolationsExceptionsCallback(Exception exception, int retry, int retryCount, int waitTime);
        /// <summary>
        /// Determines whether the specified exception is a sharing violation exception.
        /// </summary>
        /// <param name="exception">The exception. May not be null.</param>
        /// <returns>
        /// <c>true</c> if the specified exception is a sharing violation exception; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSharingViolation(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            return exception.HResult == -2147024891;
        }
    }
}
