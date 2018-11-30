using System.Diagnostics;

using YamlDotNet.Serialization;

namespace ImpersonationTester
{
    public class RunInfo
    {
        public ProcessStartupInfo StartInfo { get; set; }

        public SecurityContext IdentityRunAs { get; set; }
        [YamlIgnore]
        internal bool HasIdentityRunAs { get { return IdentityRunAs != null; } }

        public SecurityContext ProcessRunAs { get; set; }
        [YamlIgnore]
        internal bool HasProcessRunAs { get { return ProcessRunAs != null; } }

        public static void CreateSample()
        {
            RunInfo runInfo = new RunInfo
            {
                StartInfo = new ProcessStartupInfo
                {
                    FileName = "powershell.exe",
                    Arguments = @"-ExecutionPolicy Bypass -File C:\Temp\sample.ps1 -p1 aaa -p2 bbb -p3 ccc",
                    WorkingDirectory = @"C:\Temp",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                },

                IdentityRunAs = new SecurityContext
                {
                    Domain = "domain",
                    UserName = "user0",
                    Password = "password0",
                    UseIdentity = false
                },

                ProcessRunAs = new SecurityContext
                {
                    Domain = "domain",
                    UserName = "user1",
                    Password = "password1",
                    UseIdentity = true
                }
            };

            runInfo.Serializefile( "sample.yaml" );
        }

        public void Serializefile(string path)
        {
            YamlHelpers.SerializeFile( path, this, emitDefaultValues: true );
        }

        public static RunInfo DeserializeFile(string path)
        {
            return YamlHelpers.DeserializeFile<RunInfo>( path );
        }
    }
}