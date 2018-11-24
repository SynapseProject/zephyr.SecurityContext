namespace zephyr.SecurityContext.Windows
{
    public enum LogonProvider
    {
        /// <summary>
        /// Use the standard logon provider for the system.
        /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name
        /// is not in UPN format. In this case, the default provider is NTLM.
        /// NOTE: Windows 2000/NT:   The default security provider is NTLM.
        /// </summary>
        //  LOGON32_PROVIDER_DEFAULT = 0,
        //  LOGON32_PROVIDER_WINNT35 = 1,
        //  LOGON32_PROVIDER_WINNT40 = 2,
        //  LOGON32_PROVIDER_WINNT50 = 3

        Default = 0,
        WinNT35 = 1,
        WinNT40 = 2,
        WinNT50 = 3
    }
}