using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LastTab.Entities
{
    public sealed class AppCodeToken
    {
        public string User { get; set; }
        public string AppCode { get; set; }

        public static bool IsMatch(AppCodeToken token,IConfiguration configuration)
        {
            if (token == null) { return false; }
            return IsMatch(token.User, token.AppCode, configuration);
        }

        public static bool IsMatch(string user,string appcode, IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(user)) { return false; }            
            if (string.IsNullOrWhiteSpace(appcode)) { return false; }
            var key = configuration?.GetValue<string>("auth:key", null);
            if (string.IsNullOrWhiteSpace(key)) { key = "LastTab"; }
            var hmac = HMACSHA1.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(user));
            return string.Equals(appcode, Convert.ToHexString(hmac), StringComparison.OrdinalIgnoreCase);
        }
    }
}
