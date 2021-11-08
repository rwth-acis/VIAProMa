using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Extensions = TinCan.Extensions;

namespace DIG.GBLXAPI.Builders
{
    // ------------------------------------------------------------------------
    //  "name": {
    //       "en-US": "Patterns and Relationships"
    //     },
    //     "id": "https://gblxapi.org/skill/patterns-relationships",
    //     "description": {
    //       "en-US": "Actor has been presented or interacted in math skills related to: Patterns and Relationships"
    //     }
    //   }
    //
    // extensionType: Skill
    // extensionName: Patterns and Relationships
    // ------------------------------------------------------------------------
    public class ExtensionsBuilder
    {
        public static ExtensionsBuilder Start(JObject standardsObject) { return new ExtensionsBuilder(standardsObject); }

        private readonly JObject _standardsObject;

        private Dictionary<string, List<JToken>> _standardsMap;

        private ExtensionsBuilder(JObject standardsObject)
        {
            _standardsObject = standardsObject;

            _standardsMap = new Dictionary<string, List<JToken>>();
        }

        public ExtensionsBuilder WithStandard(string standardType, string standardName, string extensionType = null)
        {
            standardType = standardType.ToLower();
            standardName = standardName.ToLower();

            try
            {
                JToken token = _standardsObject[standardType][standardName];

                if (token == null) { throw new VocabMissingException("extension", standardName); }

                // If an extension type is specified, use that as the map key
                string key = (string.IsNullOrEmpty(extensionType))
                    ? standardType
                    : extensionType.ToLower();

                if(!_standardsMap.ContainsKey(key))
                {
                    _standardsMap.Add(key, new List<JToken>());
                }

                _standardsMap[key].Add(token);
            }
            catch (NullReferenceException)
            {
                throw new VocabMissingException("extension type", standardType);
            }

            return this;
        }

        public Extensions Build()
        {
            var extensions = new Extensions();

            foreach(string key in _standardsMap.Keys)
            {
                PackStandards(key, _standardsMap[key], ref extensions);
            }

            return extensions;
        }

        private void PackStandards(string type, List<JToken> standards, ref Extensions extensions)
        {
            type = type.ToLower();

            try
            {
                Uri extURI = new Uri((string)_standardsObject["extension"][type]["id"]);
                extensions.Add(extURI, JToken.FromObject(standards));
            }
            catch (NullReferenceException) { throw new VocabMissingException("extension type", type); }
        }
    }
}
