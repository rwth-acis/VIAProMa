using System;
using TinCan;
using Newtonsoft.Json.Linq;

namespace DIG.GBLXAPI.Builders
{
    // ------------------------------------------------------------------------
    //	"result": {
    //		"success": true,
    //		"duration": "PT1.46S",
    //		"response": "0",
    //		"completion": true
    //	},
    // ------------------------------------------------------------------------
    public class ResultBuilder
    {
        public static ResultBuilder Start() { return new ResultBuilder(); }

        private Result _result;

        private ResultBuilder()
        {
            _result = new Result();
        }

        public ResultBuilder Complete() { return WithCompletion(true); }
        public ResultBuilder Incomplete() { return WithCompletion(false); }

        public ResultBuilder WithCompletion(bool completed)
        {
            _result.completion = completed;

            return this;
        }

        public ResultBuilder Successful() { return WithSuccess(true); }
        public ResultBuilder Failed() { return WithSuccess(false); }

        public ResultBuilder WithSuccess(bool successful)
        {
            _result.success = successful;

            return this;
        }

        public ResultBuilder WithDuration(float duration)
        {
            _result.duration = TimeSpan.FromSeconds(duration);

            return this;
        }

        public ResultBuilder WithResponse(string response)
        {
            _result.response = response;

            return this;
        }

        public ResultBuilder WithScore(int? score)
        {
            _result.score = new Score { raw = score };

            return this;
        }

        public ResultBuilder WithExtensions(TinCan.Extensions extensions)
        {
            _result.extensions = extensions;

            return this;
        }

        public Result Build()
        {
            return _result;
        }

        public static implicit operator Result(ResultBuilder builder) => builder.Build();
    }
}
