namespace BookShop.ApiGateway.Utils
{
    public class Constants
    {
        public const string Invalid_Http_Verb = "Supplied Http content type is not implemented.";
        public const string Content_Type = "Content-Type";
        public const string APIContentType = "application/json";
        public const string FormUrlEncodedContentType = "application/x-www-form-urlencoded; charset=utf-8";
    }

    public static class OAuthConstants
    {
        public const string Grant_Type = "grant_Type";
        public const string Grant_Type_Password = "password";
        public const string Grant_Type_Client_Credentials = "client_credentials";
        public const string Bearer = "bearer";
        public const string Invalid_Credentials = "invalid credentials";
        public const string UserID = "UsrID";
        public const string GeneralSystemFailureMsg = "General System Failure";
        public const string GeneralSystemFailureErrCode = "99";
        public const string UnAuthorizedAccessMsg = "Authorization has been denied for this request.";
        public const string MissingRequiredFieldsFailureCode = "3";
        public const string MissingRequiredFieldsFailureMessage = "Missing required fields";
        public const string InvalidDataCode = "4";
        public const string InvalidDataDescription = "Invalid Data";
        public const string ServerError = "Server Error";
        public const string InActiveUserMsg = "Inactive User";
        public const string UserDeactivate = "User is Deactivated Please Contact Administrator";
        public const string NoUserFoundMsg = "No User Found With Given Combination of UserName and Password";
        public const string ErrMessageYES = "YES";
    }
}
