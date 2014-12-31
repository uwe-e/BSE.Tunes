using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE.Tunes.WebApi.Configuration
{
    public class Settings
    {
        /// <summary>
        /// Gets the setting elements of the BSEtunes section
        /// </summary>
        public static TunesConfiguration TunesConfiguration
        {
            get
            {
                return System.Configuration.ConfigurationManager.GetSection("bsetunes.configuration") as TunesConfiguration;
            }
        }
        /// <summary>
        /// Gets the impersonation credentials for accessing the audio files in the filesystem. 
        /// </summary>
        public static ImpersonationUser ImpersonationUser
        {
            get
            {
                return TunesConfiguration.ImpersonationUser;
            }
        }
    }
}