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
        /// 
        /// Adaption of https://github.com/w00w00/auto-extractor-net/blob/master/%20auto-extractor-net/AutoExtractor/IOUtils.cs
        /// 
        /// </summary>
        /// <param name="action">The action to execute. May not be null.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="waitTime">The wait time in milliseconds.</param>
        /// <returns></returns>
        public static async Task WrapSharingViolations(Func<Task> action, int retryCount = 5, int waitTime = 40)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            bool isSharingViolation = false;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    await action();
                    return;
                }
                catch (AggregateException ae)
                {
                    if (ae.Flatten().InnerExceptions.Select(ex => ex.IsSharingViolation()).Any() && i < retryCount - 1)
                    {
                        isSharingViolation = true;
                    }
                    else
                    {
                        throw ae;
                    }
                }
                catch (Exception exception)
                {
                    if (exception.IsSharingViolation() && i < retryCount - 1)
                    {
                        isSharingViolation = true;
                    }
                    else
                    {
                        throw exception;
                    }
                }
                if (isSharingViolation)
                {
                    await Task.Delay(waitTime);
                }
            }
        }
        ///// <summary>
        ///// Wraps sharing violations that could occur on a file IO operation.
        ///// </summary>
        ///// <param name="action">The action to execute. May not be null.</param>
        ///// <param name="exceptionsCallback">The exceptions callback. May be null.</param>
        ///// <param name="retryCount">The retry count.</param>
        ///// <param name="waitTime">The wait time in milliseconds.</param>
        //public static async Task WrapSharingViolations(WrapSharingViolationsCallback action, WrapSharingViolationsExceptionsCallback exceptionsCallback = null, int retryCount = 10, int waitTime = 20)
        //{
        //    if (action == null)
        //    {
        //        throw new ArgumentNullException("action");
        //    }
        //    bool isSharingViolation = false;

        //    for (int i = 0; i < retryCount; i++)
        //    {
        //        try
        //        {
        //            action();
        //            return;
        //        }
        //        catch (AggregateException ae)
        //        {
        //            var sharingViolation = ae.Flatten().InnerExceptions.Select(ex => ex.IsSharingViolation());
        //            if (ae.Flatten().InnerExceptions.Select(ex => ex.IsSharingViolation()).Any() && i < retryCount - 1)
        //            {
        //                isSharingViolation = true;
        //            }
        //            else
        //            {
        //                throw ae;
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            if (exception.IsSharingViolation() && i < retryCount - 1)
        //            {
        //                isSharingViolation = true;
        //            }
        //            else
        //            {
        //                throw exception;
        //            }
        //        }
        //        if (isSharingViolation)
        //        {
        //            await Task.Delay(waitTime);
        //        }
        //    }
        //}
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
