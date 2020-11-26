using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.CustomeSession
{
    /// <summary>
    /// static method for Isession interface
    /// To store CLR object in json format in session state
    /// </summary>
    public static class CustomeSessionExtension
    {
        public static void SetSessionData<T>(this ISession session,string key,T value)
        {

            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetSessionData<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
            {
                return default(T);
            }else
            {

                return JsonConvert.DeserializeObject<T>(data);
            }
        }
    }
}
