using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using BookShop.ApiGateway.Models;

namespace BookShop.ApiGateway.Utils
{
    public static class Utilities
    {
        // Getting encryptedKey value from Environment variables
        private static string encryptKey = Environment.GetEnvironmentVariable("ENCRYPT_KEY");

        /// <summary>Gets the value from list.</summary>
        /// <param name="key">The key.</param>
        /// <param name="keyValues">The key values.</param>
        /// <returns> </returns>
        public static string? GetValueFromList(string key, List<KeyValuePair<string, StringValues>> keyValues)
        {
            string keyExists = keyValues.FirstOrDefault(x => x.Key.Equals(key)).Key;
            if (keyExists == null || string.IsNullOrWhiteSpace(keyExists))
            {
                return null;
            }

            return Convert.ToString(keyValues.FirstOrDefault(x => x.Key.Equals(key)).Value);
        }

        /// <summary>Formats the request.</summary>
        /// <param name="request">The request.</param>
        /// <returns>Formatted Request</returns>
        public static async Task<string> FormatRequest(HttpRequest request)
        {
            // Leave the body open so the next middleware can read it.
            using var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            // Do some processing with body
            var formattedRequest = $"{request.Scheme}://{request.Host}{request.Path} {request.QueryString} {(request.Path.HasValue && request.Path.Value.Contains("Token") ? string.Empty : body)}";

            // Reset the request body stream position so the next middleware can read it
            request.Body.Position = 0;

            return formattedRequest;
        }

        /// <summary>Formats the response.</summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <returns>Formatted Response</returns>
        public static async Task<string> FormatResponse(HttpRequest request, HttpResponse response, bool isLogging)
        {
            if (response.ContentLength > 0 || response.Body?.Length > 0)
            {
                // We need to read the response stream from the beginning
                response.Body.Seek(0, SeekOrigin.Begin);
                //response.Body.Position = 0;

                // and copy it into a string
                string text = await new StreamReader(response.Body).ReadToEndAsync();

                // We need to reset the reader for the response so that the client can read it.
                response.Body.Seek(0, SeekOrigin.Begin);
                //response.Body.Position = 0;

                // Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
                return isLogging ? $"{response.StatusCode}: {(request.Path.HasValue && request.Path.Value.Contains("Token") ? string.Empty : text)}"
                    : text;
            }

            return string.Empty;
        }

        /// <summary>Gets the entity data.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="JSON">The json.</param>
        /// <returns>Return Entity based Deserialized Object Model</returns>
        public static T? GetEntityData<T>(string key, string JSON)
        {
            var jObject = string.IsNullOrEmpty(key) ? JObject.Parse(JSON) : JObject.Parse(JSON).GetValue(key);
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }


        /// <summary>Encryptors the specified value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>Encrypted Value</returns>
        public static string Encryptor(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(encryptKey);

            var inputArray = Encoding.UTF8.GetBytes(value);
            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Key = Encoding.UTF8.GetBytes(encryptKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;

            var cTransform = tripleDES.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            var encryptedValue = Convert.ToBase64String(resultArray, 0, resultArray.Length);

            return encryptedValue;
        }

        /// <summary>Decryptors the specified encrypted value.</summary>
        /// <param name="encryptedValue">The encrypted value.</param>
        /// <returns>Decrypted Value</returns>
        public static string Decryptor(string encryptedValue)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(encryptKey);

            var inputArray = Convert.FromBase64String(encryptedValue);
            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Key = Encoding.UTF8.GetBytes(encryptKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;

            var cTransform = tripleDES.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            var decryptedValue = Encoding.UTF8.GetString(resultArray);

            return decryptedValue;
        }

        /// <summary>Loads the .env file and adds to Environment Variable.</summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="System.IO.FileNotFoundException">The file '{filePath}' does not exist.</exception>
        public static void LoadEnvFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file '{filePath}' does not exist.");

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue; // Skip empty lines and comments

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue; // Skip lines that are not key-value pairs

                var key = parts[0].Trim();
                var value = parts[1].Trim();
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        /// <summary>
        /// Provides the ContentResult Response
        /// </summary>
        /// <param name="message">Accepts the message to return into response</param>
        /// <param name="HttpStatusCode">Accepts the HttpStatusCode</param>
        /// <returns>IActionResult,ContentResult</returns>
        public static IActionResult ResponseContent(string message, int HttpStatusCode)
        {
            return new ContentResult()
            {
                Content = message,
                ContentType = Constants.APIContentType,
                StatusCode = HttpStatusCode
            };
        }

        /// <summary>
        /// Method for system error
        /// </summary>
        /// <returns>SystemError Response</returns>
        public static CResponse SystemError()
        {
            var response = new CResponse
            {
                APIResponseCode = OAuthConstants.GeneralSystemFailureErrCode,
                APIReasonDescription = OAuthConstants.GeneralSystemFailureMsg,
                APIResponseDateTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                APIUniqueReferenceID = Guid.NewGuid().ToString()
            };
            return response;
        }

        /// <summary>
        /// Method for Missing required field.
        /// </summary>
        /// <returns>MissingRequiredField Response</returns>
        public static CResponse MissingRequiredField()
        {
            var response = new CResponse
            {
                APIResponseCode = OAuthConstants.MissingRequiredFieldsFailureCode,
                APIReasonDescription = OAuthConstants.MissingRequiredFieldsFailureMessage,
                APIResponseDateTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                APIUniqueReferenceID = Guid.NewGuid().ToString()
            };
            return response;
        }

        /// <summary>
        /// Invalid Data
        /// </summary>
        /// <returns>InvalidDataError Response</returns>
        public static CResponse InvalidDataError()
        {
            var response = new CResponse
            {
                APIResponseCode = OAuthConstants.InvalidDataCode,
                APIReasonDescription = OAuthConstants.InvalidDataDescription,
                APIResponseDateTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                APIUniqueReferenceID = Guid.NewGuid().ToString()
            };
            return response;
        }
    }
}
