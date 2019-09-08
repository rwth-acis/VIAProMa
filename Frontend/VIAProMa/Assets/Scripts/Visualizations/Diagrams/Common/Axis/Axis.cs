using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{

    public class Axis<T> : IAxis
    {
        public string Title
        {
            get;
            protected set;
        }

        public float NumericDataMin { get; private set; }

        public float NumericDataMax { get; private set; }

        public IDataConverter<T> DataConverter { get; private set; }

        public Axis(IDataConverter<T> dataConverter, T dataMin, T dataMax)
        {
            Title = "";
            DataConverter = dataConverter;
            NumericDataMin = DataConverter.ValueToFloat(dataMin);
            NumericDataMax = DataConverter.ValueToFloat(dataMax);
        }

        public Axis(IDataConverter<T> dataConverter, float numericDataMin, float numericDataMax)
        {
            Title = "";
            DataConverter = dataConverter;
            NumericDataMin = numericDataMin;
            NumericDataMax = numericDataMax;
        }

        public Axis(string title, IDataConverter<T> dataConverter, T dataMin, T dataMax) : this(dataConverter, dataMin, dataMax)
        {
            Title = title;
        }

        public List<IDisplayAxis> GeneratePossibleConfigurations(List<float> stepSequence)
        {
            // first convert the step sequence to a label sequence
            List<string> labels = new List<string>();
            for (int i=0;i<stepSequence.Count;i++)
            {
                T value = DataConverter.FloatToValue(stepSequence[i]);
                string formatedValue = DataConverter.ValueToString(value);
                labels.Add(formatedValue);
            }

            // now create all possible DisplayAxes from this
            List<IDisplayAxis> possibilities = new List<IDisplayAxis>();
            for (float fontSize = 0.1f; fontSize <= 2f; fontSize += 0.1f)
            {
                for (int i = 0; i < 2; i++)
                {
                    bool horizontalAlignment = (i == 0);
                    DisplayAxis displayAxis = new DisplayAxis(this, labels, fontSize, horizontalAlignment);
                    possibilities.Add(displayAxis);
                }
            }
            return possibilities;
        }
    }
}