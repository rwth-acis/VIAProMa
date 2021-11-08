// 20170523 Based on Activity.cs and Verb.cs
using System;
using Newtonsoft.Json.Linq;
using TinCan.Json;

namespace TinCan
{
	public class InteractionComponent : JsonModel
	{
		public String id;
		public LanguageMap description { get; set; }

		public InteractionComponent(){ }

		public InteractionComponent(JObject jobj)
		{
			if (jobj["id"] != null)
			{
				this.id = jobj.Value<String>("id");
			}
			if (jobj["description"] != null)
			{
				this.description = (LanguageMap)jobj.Value<JObject>("description");
			}
		}

		public override JObject ToJObject(TCAPIVersion version)
		{
			JObject result = new JObject();

			if (this.id != null)
			{
				result.Add("id", this.id);
			}
			if (this.description != null && !this.description.isEmpty())
			{
				result.Add("description", this.description.ToJObject(version)); // should be a language map
			}

			return result;
		}

		public static explicit operator InteractionComponent(JObject jobj)
		{
			return new InteractionComponent(jobj);
		}
	}

}

