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
            {"gitHubClientIdDev", "placeholder"},
            {"gitHubClientSecretDev", "placeholder"}
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