using Newtonsoft.Json;
using System.Reflection;

namespace BookShop.ApiGateway.Providers
{
    /// <summary>
    /// Logs Exception/Information in a file
    /// </summary>
    public static class FileLogger
    {
        /// <summary>Get Inner Exception Message</summary>
        /// <param name="ex">Exception ex.</param>
        /// <returns>Exception Message</returns>
        private static string? GetInnerErrorMessage(Exception ex)
        {
            return ex?.InnerException?.InnerException?.Message ?? ex?.Message;
        }

        #region Methods to write log in a file

        /// <summary>
        /// Adds the Log to the file
        /// </summary>
        /// <param name="serviceUrl">Accepts the Service Url</param>
        /// <param name="methodName">Accepts the MethodName</param>
        /// <param name="refId">Accepts the reference TableId </param>
        /// <param name="ex">Accepts the ex object</param>
        /// <param name="requestData"></param>
        /// <returns>Save, the exception Log File</returns>
        public static bool WriteLog(string serviceUrl, string methodName, string refId, Exception ex, object requestData, ILogger logger)
        {
            string? exception = ex != null ? GetInnerErrorMessage(ex) : string.Empty;
            string? exceptionType = ex?.GetType().ToString();
            string? exceptionTrace = ex?.StackTrace;
            string? appName = Assembly.GetExecutingAssembly().GetName().Name;
            string errorDetail = $"ApplicationName : {appName}, ServiceUrl : {serviceUrl}, HttpMethod : {methodName}, " +
                        $"CreationDate :{DateTime.Now}, \nRefID: {refId}\n" +
                        $"RefData : {JsonConvert.SerializeObject(requestData)}\n" +
                        $"ExceptionType : {exceptionType}, ExceptionMessage : {exception}, ExceptionTrace : {exceptionTrace}\n";
            try
            {
                logger.LogError(errorDetail);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Writes the information log in a file
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="message">The message.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>Bool</returns>
        public static bool WriteInformationLog(string log, ILogger logger)
        {
            try
            {
                logger.LogInformation(log);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Writes the information log in a file
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="message">The message.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>Bool</returns>
        public static bool WriteCustomInformationLog(string methodName, string message, DateTime timestamp, ILogger logger)
        {
            string informaton = "Method Name: " + methodName + ", Message: " + message + ", Timestamp: " + timestamp;
            try
            {
                logger.LogInformation(informaton);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
