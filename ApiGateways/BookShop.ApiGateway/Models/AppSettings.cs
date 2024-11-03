namespace BookShop.ApiGateway.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class AppSettings
    {
        public string RuntimeEnv { get; set; }
        public JwtToken JwtToken { get; set; } = new JwtToken();
    }

    public class JwtToken
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
        public string expHrs { get; set; }
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
