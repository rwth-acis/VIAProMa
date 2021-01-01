using i5.VIAProMa.Visualizations.Common.Data.DataConverters;
using i5.VIAProMa.Visualizations.Diagrams.Common.Axes;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Common.Data.DataSets
{
    public abstract class DataColumn<T> : IDataColumn
    {
        protected T dataMin;
        protected T dataMax;

        protected abstract IDataConverter<T> DataConverter { get; }

        public abstract DataType DataType { get; }

        public string Title { get; set; }

        public List<T> Values { get; set; }

        public int ValueCount { get => Values.Count; }

        public DataColumn(List<T> values)
        {
            Values = values;
        }

        public IAxis GenerateAxis()
        {
            if (Values.Count == 0)
            {
                Debug.LogError("Cannot create axis for no values");
                return null;
            }
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            for (int i = 0; i < Values.Count; i++)
            {
                float fValue = DataConverter.ValueToFloat(Values[i]);
                minValue = Mathf.Min(minValue, fValue);
                maxValue = Mathf.Max(maxValue, fValue);
            }
            Axis<T> axis = new Axis<T>(Title, DataConverter, minValue, maxValue);
            return axis;
        }

        public float GetFloatValue(int index)
        {
            return DataConverter.ValueToFloat(Values[index]);
        }
    }
}