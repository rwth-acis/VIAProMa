using System.Collections.Generic;

namespace i5.VIAProMa.Visualizations.Diagrams.Common.Axes
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
