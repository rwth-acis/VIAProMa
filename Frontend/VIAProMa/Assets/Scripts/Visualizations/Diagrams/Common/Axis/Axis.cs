using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{

    public abstract class Axis<T> : IAxis
    {
        public string Title
        {
            get;
            protected set;
        }

        public float NumericDataMin { get; private set; }

        public float NumericDataMax { get; private set; }

        public T DataMin { get; private set; }

        public T DataMax { get; private set; }

        public Axis()
        {
            Title = "";
        }

        public Axis(string title)
        {
            Title = title;
        }

        protected abstract float ValueToFloat(T value);

        protected abstract T FloatToValue(float f);

        protected abstract string ValueToString(T value);

        public List<IDisplayAxis> GeneratePossibleConfigurations(List<float> stepSequence)
        {
            // first convert the step sequence to a label sequence
            List<string> labels = new List<string>();
            for (int i=0;i<stepSequence.Count;i++)
            {
                T value = FloatToValue(stepSequence[i]);
                string formatedValue = ValueToString(value);
                labels.Add(formatedValue);
            }

            // now create all possible DisplayAxes from this
            List<IDisplayAxis> possibilities = new List<IDisplayAxis>();
            for (int fontSize = 20; fontSize <= 100; fontSize += 5)
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