using System.Diagnostics;

namespace ImpersonationTester
{
    public class ProcessStartupInfo
    {
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public ProcessWindowStyle WindowStyle { get; set; }
        public bool CreateNoWindow { get; set; }
        public bool RedirectStandardOutput { get; set; }
        public bool RedirectStandardError { get; set; }
        public bool UseShellExecute { get; set; }
        public int TimeoutMilliseconds { get; set; } = 10000;

        public ProcessStartInfo ToProcessStartInfo()
        {
            return new ProcessStartInfo
            {
                FileName = FileName,
                Arguments = Arguments,
                WorkingDirectory = WorkingDirectory,
                WindowStyle = WindowStyle,
                CreateNoWindow = CreateNoWindow,
                RedirectStandardOutput = RedirectStandardOutput,
                RedirectStandardError = RedirectStandardError,
                UseShellExecute = UseShellExecute
            };
        }
    }
}