using System;
using System.Collections.Generic;
using TinCan;
using Newtonsoft.Json.Linq;

namespace DIG.GBLXAPI.Builders
{
    // ------------------------------------------------------------------------
    //	"object": {
    //		"objectType": "Activity",
    //		"id": "https://dig-itgames.com/apps/exotrex2/minigame/titan/branchingquiz/4",
    //		"definition": {
    //			"type": "https://gblxapi.org/object/minigame",
    //			"name": {
    //				"en-US": "MiniGame Titan BranchingQuiz 4"
    //			},
    //			"description": {
    //				"en-US": "I'm very interested in your take of Titan's volcanoes. How do they compare to the ones we have on Earth?"
    //			},
    //			"interactionType": "choice",
    //			"correctResponsesPattern": [
    //				"0"
    //			],
    //			"choices": [
    //				{
    //					"id": "0",
    //					"description": {
    //						"en-US": "Titan's volcanoes are called cryovolcanoes because they ooze frozen or slushy mixtures of water and methane from under the moon's crust."
    //					}
    //				},
    //				{
    //					"id": "1",
    //					"description": {
    //						"en-US": "Titan's volcanoes are just like Earth's and the hot lava helps warm the surface."
    //					}
    //				},
    //				{
    //					"id": "2",
    //					"description": {
    //						"en-US": "Titan's volcanoes are called cryovolcanoes because they only emit steam and sparks of light from deep inside of Titan."
    //					}
    //				}
    //			]
    //		}
    //	},
    // REF: https://github.com/adlnet/xAPI-Spec/blob/master/xAPI-Data.md#Appendix2C
    // ------------------------------------------------------------------------
    public class ActivityDefinitionBuilder :
        ActivityDefinitionBuilder.IType,
        ActivityDefinitionBuilder.IName,
        ActivityDefinitionBuilder.IOptional
    {
        public static IType Start(JObject standardsObject) { return new ActivityDefinitionBuilder(standardsObject); }

        private readonly JObject _standardsObject;

        private ActivityDefinition _definition;

        private ActivityDefinitionBuilder(JObject standardsObject)
        {
            _standardsObject = standardsObject;
            _definition = new ActivityDefinition();
        }

        public IName WithType(string type)
        {
            try
            {
                _definition.type = new Uri((string)_standardsObject["activity"][type.ToLower()]["id"]);
            }
            catch (NullReferenceException) { throw new VocabMissingException("activity type", type); }

            return this;
        }

        public IOptional WithName(string name, string language = "en-US")
        {
            if (_definition.name == null)
            {
                _definition.name = new LanguageMap();
            }

            _definition.name.Add(language, name);

            return this;
        }

        public IOptional WithDescription(string description, string language = "en-US")
        {
            if (_definition.description == null)
            {
                _definition.description = new LanguageMap();
            }

            _definition.description.Add(language, description);

            return this;
        }

        public IOptional WithChoiceInteraction(List<InteractionComponent> choices)
        {
            _definition.interactionType = "choice";

            _definition.choices = choices;

            return this;
        }

        public IOptional WithLikertInteraction(List<InteractionComponent> scale)
        {
            _definition.interactionType = "likert";

            _definition.scale = scale;

            return this;
        }

        public IOptional WithMatchingInteraction(List<InteractionComponent> sources, List<InteractionComponent> targets)
        {
            _definition.interactionType = "matching";

            _definition.source = sources;
            _definition.target = targets;

            return this;
        }

        public IOptional WithPerformanceInteraction(List<InteractionComponent> steps)
        {
            _definition.interactionType = "performance";
            _definition.steps = steps;

            return this;
        }

        public IOptional WithSequencingInteraction(List<InteractionComponent> sequence)
        {
            _definition.interactionType = "sequencing";
            _definition.choices = sequence;

            return this;
        }

        public IOptional WithResponses(List<string> responses)
        {
            _definition.correctResponsesPattern = responses;

            return this;
        }

        public IOptional WithExtensions(TinCan.Extensions extensions)
        {
            _definition.extensions = extensions;

            return this;
        }

        public ActivityDefinition Build()
        {
            return _definition;
        }

        public interface IType
        {
            IName WithType(string type);
        }

        public interface IName
        {
            IOptional WithName(string name, string language = "en-US");
        }

        public interface IOptional
        {
            IOptional WithDescription(string description, string language = "en-US");

            IOptional WithChoiceInteraction(List<InteractionComponent> choices);
            IOptional WithLikertInteraction(List<InteractionComponent> interactions);
            IOptional WithMatchingInteraction(List<InteractionComponent> sources, List<InteractionComponent> targets);
            IOptional WithPerformanceInteraction(List<InteractionComponent> steps);
            IOptional WithSequencingInteraction(List<InteractionComponent> sequence);

            IOptional WithResponses(List<string> responses);
            IOptional WithExtensions(TinCan.Extensions extensions);

            ActivityDefinition Build();
        }
    }
}
