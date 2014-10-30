using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetBotsHostProject.Models
{
    public static class Secrets
    {
        private static readonly Dictionary<string,string> _secrets = new Dictionary<string, string>()
        {
            {"gitHubClientIdDev", "5552a9be02914c0ce490"},
            {"gitHubClientSecretDev", "125b4cab147e74afbf8ff06da2debf80edbe153a"}
        };

        public static string GetSecret(string key)
        {
            if (_secrets.ContainsKey(key))
            {
                return _secrets[key];
            }
            return null;
        }
    }
}