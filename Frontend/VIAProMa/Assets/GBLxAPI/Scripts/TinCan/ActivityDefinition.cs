/*
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
using System.Linq; // for .Select
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TinCan.Json;


namespace TinCan
{
	public class ActivityDefinition : JsonModel
	{
		public String id { get; set; }
		public Uri type { get; set; }
		public Uri moreInfo { get; set; }
		public LanguageMap name { get; set; }
		public LanguageMap description { get; set; }
		public Extensions extensions { get; set; }
		public String interactionType { get; set; }  // true-false, choice, fill-in, long-fill-in, matching, performance, sequencing, likert, numeric, other
		public List<String> correctResponsesPattern { get; set; }
		public List<InteractionComponent> choices { get; set; }
		public List<InteractionComponent> scale { get; set; }
		public List<InteractionComponent> source { get; set; }
		public List<InteractionComponent> target { get; set; }
		public List<InteractionComponent> steps { get; set; }

		public ActivityDefinition() {}

		public ActivityDefinition(StringOfJSON json): this(json.toJObject()) {}

		public ActivityDefinition(JObject jobj)
		{
			if (jobj["id"] != null)
			{
				this.id = jobj.Value<String>("id");
			}
			if (jobj["type"] != null)
			{
				this.type = new Uri(jobj.Value<String>("type"));
			}
			if (jobj["moreInfo"] != null)
			{
				this.moreInfo = new Uri(jobj.Value<String>("moreInfo"));
			}
			if (jobj["name"] != null)
			{
				this.name = (LanguageMap)jobj.Value<JObject>("name");
			}
			if (jobj["description"] != null)
			{
				this.description = (LanguageMap)jobj.Value<JObject>("description");
			}
			if (jobj["extensions"] != null)
			{
				this.extensions = (Extensions)jobj.Value<JObject>("extensions");
			}
			if (jobj["interactionType"] != null)
			{
				this.interactionType = jobj.Value<String>("interactionType");
			}
			if (jobj["correctResponsesPattern"] != null)
			{
				this.correctResponsesPattern = ((JArray)jobj["correctResponsesPattern"]).Select(x => x.Value<String>()).ToList<String>();
			}
			if (jobj["choices"] != null)
			{
				this.choices = new List<InteractionComponent>();
				foreach (JObject jchoice in jobj["choices"])
				{
					this.choices.Add(new InteractionComponent(jchoice));
				}
			}
			if (jobj["scale"] != null)
			{
				this.scale = new List<InteractionComponent>();
				foreach (JObject jscale in jobj["scale"])
				{
					this.scale.Add(new InteractionComponent(jscale));
				}
			}
			if (jobj["source"] != null)
			{
				this.source = new List<InteractionComponent>();
				foreach (JObject jsource in jobj["source"])
				{
					this.source.Add(new InteractionComponent(jsource));
				}
			}
			if (jobj["target"] != null)
			{
				this.target = new List<InteractionComponent>();
				foreach (JObject jtarget in jobj["target"])
				{
					this.target.Add(new InteractionComponent(jtarget));
				}
			}
			if (jobj["steps"] != null)
			{
				this.steps = new List<InteractionComponent>();
				foreach (JObject jstep in jobj["steps"])
				{
					this.steps.Add(new InteractionComponent(jstep));
				}
			}
		}

		public override JObject ToJObject(TCAPIVersion version) {
			JObject result = new JObject();

			if (this.id != null)
			{
				result.Add("id", this.id.ToString());
			}
			if (this.type != null)
			{
				result.Add("type", this.type.ToString());
			}
			if (this.moreInfo != null)
			{
				result.Add("moreInfo", this.moreInfo.ToString());
			}
			if (this.name != null && ! this.name.isEmpty())
			{
				result.Add("name", this.name.ToJObject(version));
			}
			if (this.description != null && ! this.description.isEmpty())
			{
				result.Add("description", this.description.ToJObject(version));
			}
			if (this.extensions != null && ! this.extensions.isEmpty())
			{
				result.Add("extensions", extensions.ToJObject(version));
			}
			if (this.interactionType != null)
			{
				result.Add("interactionType", this.interactionType);
			}
			if (this.correctResponsesPattern != null && this.correctResponsesPattern.Any())
			{
				result.Add("correctResponsesPattern", JToken.FromObject(this.correctResponsesPattern));
			}
			if (this.choices != null && this.choices.Any())
			{
				var jchoices = new JArray();
				result.Add("choices", jchoices);

				foreach (InteractionComponent choice in this.choices)
				{
					jchoices.Add(choice.ToJObject(version));
				}
			}
			if (this.scale != null && this.scale.Any())
			{
				var jscale = new JArray();
				result.Add("scale", jscale);

				foreach (InteractionComponent sc in this.scale)
				{
					jscale.Add(sc.ToJObject(version));
				}
			}
			if (this.source != null && this.source.Any())
			{
				var jsource = new JArray();
				result.Add("source", jsource);

				foreach (InteractionComponent src in this.source)
				{
					jsource.Add(src.ToJObject(version));
				}
			}
			if (this.target != null && this.target.Any())
			{
				var jtarget = new JArray();
				result.Add("target", jtarget);

				foreach (InteractionComponent tar in this.target)
				{
					jtarget.Add(tar.ToJObject(version));
				}
			}
			if (this.steps != null && this.steps.Any())
			{
				var jsteps = new JArray();
				result.Add("steps", jsteps);

				foreach (InteractionComponent stp in this.steps)
				{
					jsteps.Add(stp.ToJObject(version));
				}
			}

			return result;
		}

		public static explicit operator ActivityDefinition(JObject jobj)
		{
			return new ActivityDefinition(jobj);
		}
	}
}
