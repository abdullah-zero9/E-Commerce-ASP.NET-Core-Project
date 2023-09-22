using Newtonsoft.Json;

namespace work_01_Session
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session,string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObject<T>(this ISession session,string key)
        {
            var value=session.GetString(key);
            if(value == null)
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
        }
    }
}
