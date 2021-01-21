using i5.VIAProMa.Visualizations.Diagrams.Common.Axes;

namespace i5.VIAProMa.Visualizations.Common.Data.DataSets
{
    public interface IDataColumn
    {
        IAxis GenerateAxis();

        float GetFloatValue(int index);

        int ValueCount { get; }
    }
}