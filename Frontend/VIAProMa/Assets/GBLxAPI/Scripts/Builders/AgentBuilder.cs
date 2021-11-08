using System;
using TinCan;

namespace DIG.GBLXAPI.Builders
{
    // ------------------------------------------------------------------------
    //	"actor": {
    //		"objectType": "Agent",
    //		"account": {
    //			"name": "f1cd58aa-ad22-49e5-8567-d59d97d3b209",
    //			"homepage": "https://dig-itgames.com/"
    //		}
    //		"name": "John Doe"
    //	}
    //
    //	UUID: f1cd58aa-ad22-49e5-8567-d59d97d3b209
    // 	homepage: https://dig-itgames.com
    // 	name (optional): John Doe
    // ------------------------------------------------------------------------
    public class AgentBuilder : AgentBuilder.IInverseIdentifier, AgentBuilder.IOptional
    {
        public static IInverseIdentifier Start() { return new AgentBuilder(); }

        private Agent _agent;

        private AgentBuilder()
        {
            _agent = new Agent();
        }

        public IOptional WithAccount(string name, string homePage)
        {
            return WithAccount(name, new Uri(homePage));
        }

        public IOptional WithAccount(string name, Uri homePage)
        {
            return WithAccount(new AgentAccount(homePage, name));
        }

        public IOptional WithAccount(AgentAccount account)
        {
            _agent.account = account;

            return this;
        }

        public IOptional WithMbox(string mboxIRI)
        {
            _agent.mbox = mboxIRI;

            return this;
        }

        public IOptional WithName(string name)
        {
            _agent.name = name;

            return this;
        }

        public Agent Build()
        {
            return _agent;
        }

        public interface IInverseIdentifier
        {
            IOptional WithAccount(AgentAccount account);
            IOptional WithAccount(string name, Uri homePage);
            IOptional WithAccount(string name, string homePage);
            IOptional WithMbox(string mbox);
        }

        public interface IOptional
        {
            IOptional WithName(string name);
            Agent Build();
        }
    }
}
