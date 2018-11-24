using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace zephyr.SecurityContext.Windows
{
    public class Win32Identity
    {
        [DllImport( "advapi32.dll", SetLastError = true )]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain,
            string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
        public extern static bool CloseHandle(IntPtr handle);

        private IntPtr _token;
        private WindowsImpersonationContext _impContext = null;

        [PermissionSet( SecurityAction.Demand, Name = "FullTrust" )]
        public void Impersonate(SecureString userName, SecureString domain, SecureString password,
            LogonType logonType = LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider logonProvider = LogonProvider.LOGON32_PROVIDER_DEFAULT)
        {
            if( !IsImpersonating )
            {
                _token = IntPtr.Zero;

                bool ok = LogonUser( userName.ToUnsecureString(), domain.ToUnsecureString(), password.ToUnsecureString(),
                   (int)logonType, (int)logonProvider, ref _token );

                if( !ok )
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Win32Exception( error );
                }

                WindowsIdentity identity = new WindowsIdentity( _token );
                _impContext = identity.Impersonate();
            }
        }

        [PermissionSet( SecurityAction.Demand, Name = "FullTrust" )]
        public void Undo()
        {
            if( IsImpersonating )
            {
                _impContext.Undo();

                if( _token != IntPtr.Zero )
                    CloseHandle( _token );

                _impContext = null;
            }
        }

        public bool IsImpersonating { get { return _impContext != null; } }
    }
}