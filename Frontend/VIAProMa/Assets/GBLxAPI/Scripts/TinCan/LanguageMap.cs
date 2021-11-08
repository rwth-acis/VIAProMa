﻿/*
    Copyright 2014 Rustici Software

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TinCan.Json;

namespace TinCan
{
    public class LanguageMap : JsonModel
    {
        private Dictionary<String, String> map;

        public LanguageMap() {
            map = new Dictionary<String, String>();
        }
        public LanguageMap(Dictionary<String, String> map)
        {
            this.map = map;
        }

        public LanguageMap(StringOfJSON json) : this(json.toJObject()) { }
        public LanguageMap(JObject jobj) : this()
        {
            foreach (KeyValuePair<String,JToken> entry in jobj) {
                map.Add(entry.Key, (String)entry.Value);
            }
        }

        public override JObject ToJObject(TCAPIVersion version)
        {
            JObject result = new JObject();
            foreach (KeyValuePair<String, String> entry in this.map)
            {
                result.Add(entry.Key, entry.Value);
            }

            return result;
        }

        public Boolean isEmpty()
        {
            return map.Count > 0 ? false : true;
        }

        public void Add(String lang, String value)
        {
            this.map.Add(lang, value);
        }

        public static explicit operator LanguageMap(JObject jobj)
        {
            return new LanguageMap(jobj);
        }
    }
}
