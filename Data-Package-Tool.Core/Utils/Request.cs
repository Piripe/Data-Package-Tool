using DataPackageTool.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Utils
{
    public static class Request
    {
        public static async Task<string?> GetStringFromSources(this DataPackage dp, DataSourceUsability neededUsability, IList<DRequest> requests, Func<string, HttpResponseMessage, bool>? predicate = null)
        {
            return await dp.GetObjectFromSources(neededUsability, requests, new RepeatingList<Func<string, string>>(x=>x), predicate);
        }
        public static Task<T?> GetObjectFromSources<T>(this DataPackage dp, DataSourceUsability neededUsability, IList<DRequest> requests, IList<Func<HttpResponseMessage, T>> converters, Func<T, HttpResponseMessage, bool>? predicate = null)
        {
            return GetObjectFromSources(dp, neededUsability, requests, converters, predicate, null);
        }
        public static Task<T?> GetObjectFromSources<T>(this DataPackage dp, DataSourceUsability neededUsability, IList<DRequest> requests, IList<Func<string, T>> converters, Func<T, HttpResponseMessage, bool>? predicate = null)
        {
            return GetObjectFromSources(dp, neededUsability, requests, converters, predicate, res => res.Content.ReadAsStringAsync());
        }
        private static async Task<T?> GetObjectFromSources<T,U>(DataPackage dp, DataSourceUsability neededUsability, IList<DRequest> requests, IList<Func<U,T>> converters, Func<T,HttpResponseMessage,bool>? predicate,Func<HttpResponseMessage, Task<U>>? mainConverter)
        {
            for (var i = 0; i < requests.Count; i++)
            {
                DRequest dreq = requests[i];
                switch (dreq.Context)
                {
                    case DRequestContext.Bot:
                        if (dp.BotUsability < neededUsability) continue;
                        break;
                    case DRequestContext.User:
                        if (dp.SelfbotUsability <  neededUsability) continue;
                        break;
                    case DRequestContext.Invite:
                        if (dp.InviteUsability < neededUsability) continue;
                        break;
                }

                Func<U, T> converter = converters[i];

                var res = await DRequest.RequestAsync(dreq);
                T convertedRes;
                if (!res.IsSuccessStatusCode) continue;
                if (typeof(U).Equals(typeof(HttpResponseMessage)))
                {
                    convertedRes = converter.Invoke((U)Convert.ChangeType(res, typeof(U)));
                }else
                {
                    if (mainConverter == null) return default;
                    convertedRes = converter.Invoke(await mainConverter(res));
                }

                if (predicate == null || predicate.Invoke(convertedRes, res)) return convertedRes;
            }
            return default;
        }
    }
}
