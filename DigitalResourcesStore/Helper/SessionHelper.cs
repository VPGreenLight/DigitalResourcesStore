using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace DigitalResourcesStore.Helper
{
    public static class SessionHelper
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value, _jsonSerializerSettings));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetDecimal(this ISession session, string key, decimal value)
        {
            session.SetString(key, value.ToString());
        }

        public static decimal GetDecimal(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? 0 : decimal.Parse(value);
        }
    }
}
