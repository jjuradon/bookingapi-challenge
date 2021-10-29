using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Booking.API.Tests.Base
{
    internal static class HttpHelper
    {
        public static StringContent ToStringContent(this object obj)
            => new StringContent(JsonSerializer.Serialize(obj), Encoding.Default, "application/json");
    }
}