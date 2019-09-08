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

        public bool Horizontal { get; set; }

        public float NumericDataMin { get; private set; }

        public float NumericDataMax { get; private set; }

        public IDataConverter<T> DataConverter { get; private set; }

        public Axis(IDataConverter<T> dataConverter, float numericDataMin, float numericDataMax)
        {
            Title = "";
            DataConverter = dataConverter;
            NumericDataMin = numericDataMin;
            NumericDataMax = numericDataMax;
        }

        public Axis(string title, IDataConverter<T> dataConverter, float dataMin, float dataMax) : this(dataConverter, dataMin, dataMax)
        {
            Title = title;
        }

        public List<IDisplayAxis> GeneratePossibleConfigurations(float minTextSize, float maxTextSize, List<float> stepSequence)
        {
            // first convert the step sequence to a label sequence
            List<string> labels = new List<string>();
            for (int i=0;i<stepSequence.Count;i++)
            {
                T value = DataConverter.FloatToValue(stepSequence[i]);
                string formattedValue = DataConverter.ValueToString(value);
                labels.Add(formattedValue);
            }

            // now create all possible DisplayAxes from this
            List<IDisplayAxis> possibilities = new List<IDisplayAxis>();
            for (float fontSize = minTextSize; fontSize <= maxTextSize; fontSize += 0.1f)
            {
                for (int i = 0; i < 2; i++)
                {
                    bool horizontalAlignment = (i == 0);
                    DisplayAxis displayAxis = new DisplayAxis(this, labels, fontSize, horizontalAlignment, Horizontal);
                    possibilities.Add(displayAxis);
                }
            }
            return possibilities;
        }
    }
}