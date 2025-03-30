using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace CnSharp.Localization
{
    public static class LocalizationHelper
    {
        private static readonly ConcurrentDictionary<string, string> Cache =
            new ConcurrentDictionary<string, string>();

        public static string GetLocalText(string key, string resourceNameSpace, Assembly assembly)
        {
            var ci = CultureInfo.CurrentUICulture;
            var cultureName = ci.Name;
            return GetLocalText(key, resourceNameSpace, assembly, cultureName);
        }

        public static string GetLocalText(string key, string resourceNameSpace, Assembly assembly, string cultureName)
        {
            var resourceKey = $"{assembly.FullName}.{cultureName}.{key}";
            if (Cache.TryGetValue(resourceKey, out var value))
            {
                return value;
            }

            ResourceManager rm;
            try
            {
                rm = new ResourceManager($"{resourceNameSpace}.{cultureName}", assembly);
            }
            catch
            {
                rm = new ResourceManager($"{resourceNameSpace}.en-us", assembly);
            }

            var str = rm.GetString(key);
            Cache.TryAdd(resourceKey, str);
            return str;
        }
        
    }
}