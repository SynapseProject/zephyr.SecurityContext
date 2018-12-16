using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace Zephyr.SecurityContext.Windows
{
    public class Win32Identity : IDisposable
    {
        [DllImport( "advapi32.dll", SetLastError = true )]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain,
            string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
        public extern static bool CloseHandle(IntPtr handle);

        private IntPtr _token;

        public WindowsIdentity Identity { get; internal set; } = null;
        public bool HasIdentity { get { return Identity != null; } }

        public Win32Identity() { }
        public Win32Identity(SecureString userName, SecureString domain, SecureString password,
            LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            Logon( userName, domain, password, logonType, logonProvider );
        }

        [PermissionSet( SecurityAction.Demand, Name = "FullTrust" )]
        public WindowsIdentity Logon(SecureString userName, SecureString domain, SecureString password,
            LogonType logonType = LogonType.Interactive, LogonProvider logonProvider = LogonProvider.Default)
        {
            if( !HasIdentity )
            {
                _token = IntPtr.Zero;

                bool ok = LogonUser( userName.ToUnsecureString(), domain.ToUnsecureString(), password.ToUnsecureString(),
                   (int)logonType, (int)logonProvider, ref _token );

                if( !ok )
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Win32Exception( error );
                }

                Identity = new WindowsIdentity( _token );
            }

            return Identity;
        }

        [PermissionSet( SecurityAction.Demand, Name = "FullTrust" )]
        public void Logoff()
        {
            if( _token != IntPtr.Zero )
            {
                CloseHandle( _token );
                _token = IntPtr.Zero;
            }

            Identity?.Dispose();
            Identity = null;
        }

        public void Dispose()
        {
            Logoff();
        }
    }
}