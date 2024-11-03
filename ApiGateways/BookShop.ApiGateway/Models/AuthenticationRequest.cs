namespace BookShop.ApiGateway.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Class for Authentication Request
    /// </summary>
    public class AuthenticationRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
