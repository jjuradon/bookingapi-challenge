using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Booking.API.Data;
using Booking.API.Tests.Fixture;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Booking.API.Tests.Base
{
    public enum TestMethods
    {
        GET,
        POST,
        PUT,
        PATCH,
        DELETE
    }

    public abstract class EndpointTestsBase : IClassFixture<BookingApiFactory> {
        protected readonly BookingApiFactory _factory;
        protected readonly HttpClient _client;
        protected readonly JsonSerializerOptions _options;
        protected readonly BookingContext _db;

        /// <summary>
        /// Endpoint URI withput route parameters
        /// </summary>
        public string URI { get => $"api/{ENDPOINT}"; }

        public abstract string ENDPOINT { get; }

        protected virtual string NOT_FOUND_MESSAGE { get; } = "{0} not found.";

        public EndpointTestsBase(BookingApiFactory factory) {
            _factory = factory;
            _client = _factory.CreateClient();
            _db = _factory.Db;
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// GET request for test client
        /// </summary>
        /// <param name="parameter">Endpoint route parameter</param>
        /// <returns>Server response formated in a HttpResponseMessage object </returns>
        public async Task<HttpResponseMessage> GetMethod(string parameter = "")
            => await _client.GetAsync($"{URI}/{parameter}");

        /// <summary>
        /// POST method request
        /// </summary>
        /// <param name="body">Object type representing body format for request</param>
        /// <param name="parameter">Endpoint route parameter</param>
        /// <returns>Server response formated in a HttpResponseMessage object </returns>
        public async Task<HttpResponseMessage> PostMethod(object body, string parameter = "")
            => await _client.PostAsync($"{URI}/{parameter}", body.ToStringContent());

        /// <summary>
        /// PUT method request
        /// </summary>
        /// <param name="body">Object type representing body format for request</param>
        /// <param name="parameter">Endpoint route parameter</param>
        /// <returns>Server response formated in a HttpResponseMessage object </returns>
        public async Task<HttpResponseMessage> PutMethod(object body, string parameter = "")
            => await _client.PutAsync($"{URI}/{parameter}", body.ToStringContent());

        /// <summary>
        /// DELETE request for test client.
        /// </summary>
        /// <param name="parameter">Endpoint route parameter.</param>
        /// <returns>Server response formated in a HttpResponseMessage object. </returns>
        public async Task<HttpResponseMessage> DeleteMethod(string parameter = "")
            => await _client.DeleteAsync($"{URI}/{parameter}");


        #region Verification Methods

        /// <summary>
        /// Verifies if the response is a valid Success response (200-299).
        /// </summary>
        /// <param name="response">Object with server response.</param>
        /// <param name="expectedCode">Status Code expected for server response.</param>
        /// <returns></returns>
        protected async Task<string> VerifySuccessResponse(HttpResponseMessage response, int expectedCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}: {content}");
            var actualCode = response.StatusCode.GetHashCode();
            Assert.Equal(expectedCode, actualCode);

            if (actualCode != StatusCodes.Status204NoContent)
            {
                Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
                Assert.NotEmpty(content);
            }
            return content;
        }

        /// <summary>
        /// Verifies if the response is a valid Error response (400-499).
        /// </summary>
        /// <param name="response">Object with server response.</param>
        /// <param name="expectedCode">Status Code expected for server response.</param>
        /// <returns></returns>
        protected async Task<string> VerifyErrorResponse(HttpResponseMessage response, int expectedCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.False(response.IsSuccessStatusCode, $"{response.StatusCode}: {content}");
            var actualCode = response.StatusCode.GetHashCode();
            Assert.Equal(expectedCode, actualCode);

            Assert.NotEmpty(content);
            return content;
        }

        /// <summary>
        /// Verifies if the response is a valid Success response (200-299).
        /// </summary>
        /// <param name="expectedCode">Status Code expected for server response.</param>
        /// <returns>A service response in JSON format as string.</returns>
        protected async Task<string> VerifySuccessResponse(TestMethods method, int expectedCode, params object[] args)
        {

            HttpResponseMessage response = await ExecuteClientRequest(method, args);

            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}: {content}");
            var actualCode = response.StatusCode.GetHashCode();
            Assert.Equal(expectedCode, actualCode);

            if (actualCode != StatusCodes.Status204NoContent)
            {
                Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
                Assert.NotEmpty(content);
            }
            return content;
        }

        /// <summary>
        /// Verifies if the response is a valid Success response (200-299).
        /// </summary>
        /// <typeparam name="T">Type of response.</typeparam>
        /// <param name="method">Request method.</param>
        /// <param name="expectedCode">Status Code expected for server response.</param>
        /// <param name="args">Request arguments array.</param>
        /// <returns>A service response as object.</returns>
        protected async Task<T> VerifySuccessResponse<T>(TestMethods method, int expectedCode, params object[] args)
            => DeserializeContent<T>(await VerifySuccessResponse(method, expectedCode, args));

        /// <summary>
        /// Verifies if the response is a valid Error response (400-499).
        /// </summary>
        /// <param name="method">Request method.</param>
        /// <param name="expectedCode">Status Code expected for server response.</param>
        /// <param name="args">Request arguments array.</param>
        /// <returns>A service response dictionary with errors..</returns>
        protected async Task<string> VerifyErrorResponse(TestMethods method, int expectedCode, params object[] args)
        {
            HttpResponseMessage response = await ExecuteClientRequest(method, args);

            var content = await response.Content.ReadAsStringAsync();
            Assert.False(response.IsSuccessStatusCode, $"{response.StatusCode}: {content}");
            var actualCode = response.StatusCode.GetHashCode();
            if (expectedCode < 500) Assert.False(actualCode >= 500, $"{response.StatusCode}: {content}");
            Assert.Equal(expectedCode, actualCode);
            Assert.NotEmpty(content);
            return content;
        }


        #endregion

        #region Call Service Method
        private async Task<HttpResponseMessage> ExecuteClientRequest(TestMethods method, object[] args)
        {
            ParseArgs(args, method, out var parameter, out var body);

            return method switch
            {
                TestMethods.GET => await GetMethod(parameter),
                TestMethods.POST => await PostMethod(body, parameter),
                TestMethods.PUT => await PutMethod(body, parameter),
                TestMethods.DELETE => await DeleteMethod(parameter),
                _ => throw new NotImplementedException()
            };
        }

        private void ParseArgs(object[] args, TestMethods method, out string parameter, out object body)
        {
            parameter = string.Empty;
            body = null;

            if (args.Length > 0)
            {
                switch (method)
                {
                    case TestMethods.GET:
                    case TestMethods.DELETE:
                        parameter = GetParameter(args[0]);
                        break;
                    case TestMethods.POST:
                    case TestMethods.PUT:
                    case TestMethods.PATCH:
                        body = args[0];
                        break;
                }
            }

            if (args.Length > 1)
            {
                parameter = GetParameter(args[1]);
            }
        }

        private string GetParameter(object value)
        {
            if (!Regex.IsMatch(value.ToString(), @"\w+"))
                throw new InvalidOperationException("First argument must be a word.");

            return value.ToString();
        }
        #endregion

        #region Deserializer
        protected T DeserializeContent<T>(string content) => JsonSerializer.Deserialize<T>(content, _options);

        protected IDictionary<string, string> ErrorResponse(string content, string expected)
        {
            var dic = DeserializeContent<IDictionary<string, string>>(content);
            Assert.NotEmpty(dic);
            Assert.True(dic.Count == 1);
            Assert.Contains(dic.Keys, x => x == "error");
            var error = dic["error"];
            Assert.Equal(expected, error);
            return dic;
        }

        protected IDictionary<string, string> NotFoundErrorResponse(string content, string element)
        {
            var dic = DeserializeContent<IDictionary<string, string>>(content);
            Assert.NotEmpty(dic);
            Assert.True(dic.Count == 1);
            Assert.Contains(dic.Keys, x => x == "error");
            var error = dic["error"];
            Assert.Equal(string.Format(NOT_FOUND_MESSAGE, element), error);
            return dic;
        }

        protected IDictionary<string, IList<string>> ValidationErrorResponse(string content, string expectedContent)
        {
            var dic = DeserializeContent<IDictionary<string, IList<string>>>(content);
            var expected = DeserializeContent<IDictionary<string, IList<string>>>(expectedContent);

            Assert.NotEmpty(dic);
            Assert.Equal(expected.Keys, dic.Keys);

            foreach (var error in dic)
            {
                Assert.True(expected.ContainsKey(error.Key));
                Assert.Equal(expected[error.Key], error.Value);
            }

            return dic;
        }
        #endregion

    }
}