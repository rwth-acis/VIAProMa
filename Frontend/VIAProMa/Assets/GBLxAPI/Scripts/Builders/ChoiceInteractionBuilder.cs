using TinCan;

namespace DIG.GBLXAPI.Builders
{
    public class ChoiceInteractionBuilder :
        ChoiceInteractionBuilder.IID,
        ChoiceInteractionBuilder.IDescription,
        ChoiceInteractionBuilder.IOptional

    {
        public static IID Start() { return new ChoiceInteractionBuilder(); }

        private InteractionComponent _choice;

        private ChoiceInteractionBuilder()
        {
            _choice = new InteractionComponent();
        }

        public IDescription WithID(string id)
        {
            _choice.id = id;
            return this;
        }

        public IOptional WithDescription(string description, string language = "en-US")
        {
            if(_choice.description == null)
            {
                _choice.description = new LanguageMap();
            }

            _choice.description.Add(language, description);

            return this;
        }

        public InteractionComponent Build()
        {
            return _choice;
        }

        public interface IID
        {
            IDescription WithID(string id);
        }

        public interface IDescription
        {
            IOptional WithDescription(string description, string language = "en-US");
        }

        public interface IOptional
        {
            InteractionComponent Build();
        }
    }
}
