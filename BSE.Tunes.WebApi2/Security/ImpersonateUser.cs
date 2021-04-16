using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace BSE.Tunes.WebApi.Security
{
    public class ImpersonateUser : IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(
            String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);
        /// <summary>
        /// Use the standard logon provider for the system.
        /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name
        /// is not in UPN format. In this case, the default provider is NTLM. 
        /// </summary>
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        /// <summary>
        /// This logon type is intended for users who will be interactively using the computer, 
        /// such as a user being logged on by a terminal server, remote shell, or similar process. 
        /// This logon type has the additional expense of caching logon information for disconnected operations;
        /// therefore, it is inappropriate for some client/server applications, such as a mail server. 
        /// </summary>
        private const int LOGON32_LOGON_INTERACTIVE = 2;

        private bool _disposed = false;
        private WindowsImpersonationContext _impersonationContext;
        private SafeTokenHandle _safeTokenHandle;

        public ImpersonateUser(
            string userName,
            string domainName,
            string password)
            : this(userName, domainName, password, LOGON32_LOGON_INTERACTIVE)
        {
        }

        public ImpersonateUser(string userName, string domainName, string password, int logonType)
        {
            ImpersonateValidUser(userName, domainName, password, logonType, LOGON32_PROVIDER_DEFAULT);
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _impersonationContext?.Undo();
                _impersonationContext?.Dispose();
                _safeTokenHandle?.Dispose();
            }

            _disposed = true;
        }

        private void ImpersonateValidUser(
            string userName,
            string domain,
            string password,
            int logonType,
            int logonProvider)
        {
            try
            {
                if (LogonUser(userName, domain, password, logonType, logonProvider,
                    out _safeTokenHandle))
                {

                    _impersonationContext = WindowsIdentity.Impersonate(_safeTokenHandle.DangerousGetHandle());
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (Exception)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        
        private void UndoImpersonation()
        {
            _impersonationContext?.Undo();
            _safeTokenHandle?.Close();
        }
    }

    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
}