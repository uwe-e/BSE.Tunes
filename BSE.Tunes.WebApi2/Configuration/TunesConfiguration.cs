using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BSE.Tunes.WebApi.Configuration
{
    public class TunesConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("impersonationuser")]
        public ImpersonationUser ImpersonationUser
        {
            get
            {
                return (ImpersonationUser)this["impersonationuser"];
            }
            set
            {
                this["impersonationuser"] = value;
            }
        }
    }
    public class ImpersonationUser : ConfigurationElement
    {
        [ConfigurationProperty("username", IsRequired = true)]
        public String Username
        {
            get
            {
                return (String)this["username"];
            }
            set
            {
                this["username"] = value;
            }
        }
        [ConfigurationProperty("domain", IsRequired = true)]
        public String Domain
        {
            get
            {
                return (String)this["domain"];
            }
            set
            {
                this["domain"] = value;
            }
        }
        [ConfigurationProperty("password", IsRequired = true)]
        public String Password
        {
            get
            {
                return (String)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }
        [ConfigurationProperty("logontype", IsRequired = false, DefaultValue=2)]
        public int LogonType
        {
            get
            {
                return (int)this["logontype"];
            }
            set
            {
                this["logontype"] = value;
            }
        }
    }
}