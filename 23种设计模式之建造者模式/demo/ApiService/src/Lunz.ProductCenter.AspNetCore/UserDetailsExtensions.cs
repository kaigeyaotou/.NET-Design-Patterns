using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using Lunz.ProductCenter.Core.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lunz.ProductCenter.AspNetCore
{
    public static class UserDetailsExtensions
    {
        private static readonly Dictionary<string, string> KeyPropertyDictionary = new Dictionary<string, string>()
        {
            { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "sub" },
        };

        public static UserDetails UserDetails(this HttpRequest request)
        {
            var userDetails = request.HttpContext.User.GetUserDetails();
            return userDetails ?? request.GetUserDetails();
        }

        public static UserDetails GetUserDetails(this ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity.IsAuthenticated
                || claimsPrincipal.Claims == null)
            {
                return null;
            }

            var json = ToJson(claimsPrincipal.Claims);
            var userDetails = JsonConvert.DeserializeObject<UserDetails>(json, new UrlEncodeConverter());
            return userDetails;
        }

        public static UserDetails GetUserDetails(this HttpRequest request)
        {
            var data = request.Headers["user-details"];

            if (string.IsNullOrWhiteSpace(data) || data.Count == 0)
            {
                return null;
            }

            var userDetails = JsonConvert.DeserializeObject<UserDetails>(data[0], new UrlEncodeConverter());
            return userDetails;
        }

        private static string ToJson(IEnumerable<Claim> claims)
        {
            if (claims == null)
            {
                return "{}";
            }

            var keys = claims.Select(x => x.Type).Distinct();
            var items = keys.Select(x => ToJson(x, claims));
            return string.Concat("{", string.Join(",", items), "}");
        }

        private static string ToJson(string key, IEnumerable<Claim> claims)
        {
            var propertyName = KeyPropertyDictionary.ContainsKey(key) ? KeyPropertyDictionary[key] : key;

            var result = claims.Where(x => x.Type == key).ToList();
            if (result.Count == 0)
            {
                return $"\"{propertyName}\":null";
            }

            var value = string.Join(",", result.Select(x => $"\"{HttpUtility.UrlEncode(x.Value, Encoding.UTF8)}\""));
            return result.Count == 1 ? $"\"{propertyName}\":{value}" : $"\"{propertyName}\":[{value}]";
        }
    }
}