namespace BookShop.ApiGateway.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class NonRequiredEnpoints
    {
        public string Endpoint { get; set; }
        public string Method { get; set; }
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static class OcelotNonEndpoints
    {
        // GET - configuration
        // POST - configuration
        // DELETE - outputcache/:param
        public static List<NonRequiredEnpoints> OcelotNonRequiredEnpoints()
        {
            return new List<NonRequiredEnpoints>()
            {
                new NonRequiredEnpoints() { Endpoint = "configuration", Method = "GET" },
                new NonRequiredEnpoints() { Endpoint = "configuration", Method = "POST" },
                new NonRequiredEnpoints() { Endpoint = "outputcache", Method = "DELETE" }
            };
        }
    }
}
