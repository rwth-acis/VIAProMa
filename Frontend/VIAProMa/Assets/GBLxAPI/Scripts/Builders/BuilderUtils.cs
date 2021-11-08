using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DIG.GBLXAPI.Builders
{
    public static class BuilderUtils
    {
        public static Dictionary<string, string> ParseDictionary(JToken json)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            // Parse languages from the standards
            foreach (var child in json.Children<JProperty>())
            {
                string lang = child.Name;
                map.Add(lang, (string)json[lang]);
            }

            return map;
        }
    }
}
