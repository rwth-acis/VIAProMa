using System;
using System.Collections.Generic;
using TinCan;
using Newtonsoft.Json.Linq;

namespace DIG.GBLXAPI.Builders
{
    // ------------------------------------------------------------------------
    //	"object": {
    //		"objectType": "Activity",
    //		"id": "https://dig-itgames.com/apps/exotrex2",
    //		"definition": {
    //			"type": "https://gblxapi.org/object/serious-game",
    //			"name": {
    //				"en-US": "ExoTrex 2"
    //			}
    //		}
    //	}
    //
    //	activityURI: https://dig-itgames.com/apps/exotrex2
    //	activityType: serious-game
    //	activityName: ExoTrex 2
    // ------------------------------------------------------------------------
    public class ActivityBuilder :
        ActivityBuilder.IUri,
        ActivityBuilder.IOptional
    {
        public static IUri Start(JObject standardsObject) { return new ActivityBuilder(standardsObject); }

        private const string JActivityKey = "activity";

        private readonly JObject _standardsObject;

        private Activity _activity;
        private Dictionary<string, string> _nameDictionary;
        private string _value;

        private ActivityBuilder(JObject standardsObject)
        {
            _standardsObject = standardsObject;

            _activity = new Activity();
        }

        public IOptional WithID(string iriString) { return WithID(new Uri(iriString)); }

        public IOptional WithID(Uri iri)
        {
            _activity.id = iri;

            return this;
        }

        public IOptional WithType(string type)
        {
            try
            {
                if(_activity.definition == null)
                {
                    _activity.definition = new ActivityDefinition();
                }

                var definition = _activity.definition;

                var activityToken = _standardsObject[JActivityKey][type.ToLower()];

                definition.type = new Uri((string)activityToken["id"]);
                _nameDictionary = BuilderUtils.ParseDictionary(activityToken["name"]);
                definition.description = new LanguageMap(BuilderUtils.ParseDictionary(activityToken["description"]));

            }
            catch (NullReferenceException) { throw new VocabMissingException("activity type", type); }

            return this;
        }

        public IOptional WithValue(string value)
        {
            _value = value;

            return this;
        }

        public IOptional WithDefinition(ActivityDefinition definition)
        {
            _activity.definition = definition;

            return this;
        }

        public IOptional WithChoiceInteraction(List<InteractionComponent> choices)
        {
            if(_activity.definition == null)
            {
                _activity.definition = new ActivityDefinition();
            }

            _activity.definition.interactionType = "choice";

            _activity.definition.choices = choices;

            return this;
        }

        public IOptional WithLikertInteraction(List<InteractionComponent> scale)
        {
            if (_activity.definition == null)
            {
                _activity.definition = new ActivityDefinition();
            }

            _activity.definition.interactionType = "likert";

            _activity.definition.scale = scale;

            return this;
        }

        public IOptional WithMatchingInteraction(List<InteractionComponent> sources, List<InteractionComponent> targets)
        {
            if (_activity.definition == null)
            {
                _activity.definition = new ActivityDefinition();
            }

            _activity.definition.interactionType = "matching";

            _activity.definition.source = sources;
            _activity.definition.target = targets;

            return this;
        }

        public IOptional WithPerformanceInteraction(List<InteractionComponent> steps)
        {
            if (_activity.definition == null)
            {
                _activity.definition = new ActivityDefinition();
            }

            _activity.definition.interactionType = "performance";
            _activity.definition.steps = steps;

            return this;
        }

        public IOptional WithSequencingInteraction(List<InteractionComponent> sequence)
        {
            if (_activity.definition == null)
            {
                _activity.definition = new ActivityDefinition();
            }

            _activity.definition.interactionType = "sequencing";
            _activity.definition.choices = sequence;

            return this;
        }

        public IOptional WithResponses(List<string> responses)
        {
            if (_activity.definition == null)
            {
                _activity.definition = new ActivityDefinition();
            }

            _activity.definition.correctResponsesPattern = responses;

            return this;
        }

        public IOptional WithExtensions(TinCan.Extensions extensions)
        {
            if (_activity.definition == null)
            {
                _activity.definition = new ActivityDefinition();
            }

            _activity.definition.extensions = extensions;

            return this;
        }

        public Activity Build()
        {
            // If there's additional info attached to the activity, add it to the name
            // TODO: This would be better as an extension, but some LRS's don't support that
            if(!string.IsNullOrEmpty(_value))
            {
                List<string> keys = new List<string>(_nameDictionary.Keys);
                foreach(var key in keys)
                {
                    _nameDictionary[key] += $" - {_value}";
                }
            }

            if(_nameDictionary != null)
            {
                _activity.definition.name = new LanguageMap(_nameDictionary);
            }

            return _activity;
        }

        public interface IUri
        {
            IOptional WithID(string uriString);
            IOptional WithID(Uri id);
        }

        public interface IOptional
        {
            IOptional WithType(string type);
            IOptional WithValue(string value);
            IOptional WithDefinition(ActivityDefinition definition);

            IOptional WithChoiceInteraction(List<InteractionComponent> choices);
            IOptional WithLikertInteraction(List<InteractionComponent> interactions);
            IOptional WithMatchingInteraction(List<InteractionComponent> sources, List<InteractionComponent> targets);
            IOptional WithPerformanceInteraction(List<InteractionComponent> steps);
            IOptional WithSequencingInteraction(List<InteractionComponent> sequence);

            IOptional WithResponses(List<string> responses);
            IOptional WithExtensions(TinCan.Extensions extensions);

            Activity Build();
        }
    }
}
