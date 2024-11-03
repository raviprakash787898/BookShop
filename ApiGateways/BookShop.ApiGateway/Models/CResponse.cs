namespace BookShop.ApiGateway.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Responce Class
    /// </summary>
    public class CResponse
    {
        /// <summary>
        /// Response Code for the Request
        /// </summary>
        public string APIResponseCode { get; set; }
        /// <summary>
        /// Description of the Response Message
        /// </summary>
        public string APIReasonDescription { get; set; }
        /// <summary>
        /// Time at the API Response Generated
        /// </summary>
        public string APIResponseDateTime { get; set; }
        /// <summary>
        /// Unique Tag number for the Response
        /// </summary>
        public string APIUniqueReferenceID { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
