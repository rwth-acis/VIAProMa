using System;
using TinCan;
using Newtonsoft.Json.Linq;

namespace DIG.GBLXAPI.Builders
{
    // ------------------------------------------------------------------------
    //	"verb": {
    //		"id": "https://w3id.org/xapi/dod-isd/verbs/started",
    //		"display": {
    //			"en-US": "started"
    //		}
    //	}
    //
    // statementVerb: started
    // ------------------------------------------------------------------------
    public class VerbBuilder :
        VerbBuilder.IAction,
        VerbBuilder.IOptional
    {
        public static IAction Start(JObject standardsObject) { return new VerbBuilder(standardsObject); }

        private const string JVerbKey = "verb";

        private readonly JObject _standardsObject;

        private string _action;

        private VerbBuilder(JObject standardsObject)
        {
            _standardsObject = standardsObject;
        }

        public IOptional WithAction(string action)
        {
            _action = action.ToLower();

            return this;
        }

        public Verb Build()
        {
            Verb verb = new Verb();

            try
            {
                var verbToken = _standardsObject[JVerbKey][_action];
                verb.id = new Uri((string)verbToken["id"]);
                verb.display = new LanguageMap(BuilderUtils.ParseDictionary(verbToken["name"]));

            }
            catch (NullReferenceException)
            {
                throw new VocabMissingException("verb", _action);
            }

            return verb;
        }

        public interface IAction
        {
            IOptional WithAction(string statement);
        }

        public interface IOptional
        {
            Verb Build();
        }
    }
}
