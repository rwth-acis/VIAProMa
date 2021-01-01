using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Common.Data.DataConverters
{
    public class StringDataConverter : IDataConverter<string>
    {
        private List<string> values;

        public StringDataConverter(List<string> values)
        {
            this.values = values.Distinct().ToList();
        }

        public string FloatToValue(float f)
        {
            f = Mathf.Clamp(f, 0, values.Count-1);
            return values[Mathf.RoundToInt(f)];
        }

        public float ValueToFloat(string value)
        {
            return values.IndexOf(value);
        }

        public string ValueToString(string value)
        {
            return value;
        }
    }
}
