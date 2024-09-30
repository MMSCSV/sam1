namespace CareFusion.Dispensing.Contracts
{
    public class TokenCredentials: Credentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
    }
}
