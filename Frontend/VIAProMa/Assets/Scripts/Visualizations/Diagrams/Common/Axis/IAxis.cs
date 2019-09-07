using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public interface IAxis
    {
        string Title { get; }

        float NumericDataMin { get; }

        float NumericDataMax { get; }

        List<IDisplayAxis> GeneratePossibleConfigurations(List<float> stepSequence);
    }
}
