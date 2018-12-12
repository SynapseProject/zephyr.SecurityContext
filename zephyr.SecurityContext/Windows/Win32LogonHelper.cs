using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using Microsoft.Win32.SafeHandles;

namespace Zephyr.SecurityContext.Windows
{
    public class Win32LogonHelper : IDisposable
    {
        [DllImport( "advapi32.dll", SetLastError = true )]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain,
            string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        SafeAccessTokenHandle _token;
        public SafeAccessTokenHandle TokenHandle { get { return _token; } }

        [PermissionSet( SecurityAction.Demand, Name = "FullTrust" )]
        public SafeAccessTokenHandle Logon(SecureString userName, SecureString domain, SecureString password,
            LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            bool ok = LogonUser( userName.ToUnsecureString(), domain.ToUnsecureString(), password.ToUnsecureString(),
               (int)logonType, (int)logonProvider, out _token );

            if( !ok )
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception( error );
            }

            return _token;
        }

        [PermissionSet( SecurityAction.Demand, Name = "FullTrust" )]
        public void Logoff()
        {
            if( _token != null && !_token.IsClosed )
                _token.Close();
        }

        public void Dispose()
        {
            Logoff();
        }
    }
}