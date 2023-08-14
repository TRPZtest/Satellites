using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Satellites.Helpers
{
    public static class QueryStringHelper
    {
        public static string ToQueryString(this NameValueCollection source, bool removeEmptyEntries)
        {
            return source != null ? "?" + String.Join("&", source.AllKeys
                .Where(key => !removeEmptyEntries || source.GetValues(key).Any(value => !String.IsNullOrEmpty(value)))
                .SelectMany(key => source.GetValues(key)
                    .Where(value => !removeEmptyEntries || !String.IsNullOrEmpty(value))
                    .Select(value => String.Format("{0}={1}", HttpUtility.UrlEncode(key), value != null ? HttpUtility.UrlEncode(value) : string.Empty)))
                .ToArray())
                : string.Empty;
        }

        public static string GetMD5(this byte[] inputBytes)
        {
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }

        public static IEnumerable<List<T>> SplitList<T>(this List<T> locations, int size = 100)
        {
            for (int i = 0; i < locations.Count; i += size)
            {
                yield return locations.GetRange(i, Math.Min(size, locations.Count - i));
            }
        }
    }
}
