namespace ImpersonationTester
{
    public class SecurityContext
    {
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseIdentity { get; set; } = true;
    }
}