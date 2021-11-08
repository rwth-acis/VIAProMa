using System;
using TinCan;

namespace DIG.GBLXAPI.Builders
{
    public class StatementBuilder :
        StatementBuilder.IAgent,
        StatementBuilder.IVerb,
        StatementBuilder.IActivity,
        StatementBuilder.IOptional
    {
        public static IAgent Start() { return new StatementBuilder(); }

        private Agent _actor;
        private Verb _verb;
        private Activity _activity;
        private Context _context;
        private Result _result;

        private StatementBuilder()
        {

        }

        public IVerb WithActor(Agent actor)
        {
            _actor = actor;

            return this;
        }

        public IActivity WithVerb(string verb)
        {
            _verb = GBLXAPI.Verb.WithAction(verb).Build();
            return this;
        }

        public IActivity WithVerb(Verb verb)
        {
            _verb = verb;

            return this;
        }

        public IOptional WithTargetActivity(Activity activity)
        {
            _activity = activity;

            return this;
        }

        public IOptional WithContext(Context context)
        {
            _context = context;

            return this;
        }

        public IOptional WithResult(Result result)
        {
            _result = result;

            return this;
        }

        // TODO: Optional parameters?
        public Statement Build()
        {
            Statement statement = new Statement
            {
                actor = _actor,
                verb = _verb,
                result = _result,
                target = _activity,
                context = _context
            };

            return statement;
        }

        // TODO: Fold in enqueue better
        // TODO: Return type?
        public void Enqueue(Action<bool, string> sendCallback = null)
        {
            GBLXAPI.EnqueueStatement(Build(), sendCallback);
        }

        public interface IAgent
        {
            IVerb WithActor(Agent actor);
        }

        public interface IVerb
        {
            IActivity WithVerb(string verb);
            IActivity WithVerb(Verb verb);
        }

        public interface IActivity
        {
            IOptional WithTargetActivity(Activity activity);
        }

        public interface IOptional
        {
            IOptional WithContext(Context context);
            IOptional WithResult(Result result);

            Statement Build();

            void Enqueue(Action<bool, string> sendCallback = null);
        }
    }
}
