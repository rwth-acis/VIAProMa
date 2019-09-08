using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public interface IAxis
    {
        string Title { get; }

        bool Horizontal { get; set; }

        float NumericDataMin { get; }

        float NumericDataMax { get; }

        List<IDisplayAxis> GeneratePossibleConfigurations(float minTextSize, float maxTextSize, List<float> stepSequence);
    }
}
