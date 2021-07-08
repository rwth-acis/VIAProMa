namespace i5.VIAProMa.Visualizations.Common.Data.DataConverters
{
    public interface IDataConverter<T>
    {
        T FloatToValue(float f);

        float ValueToFloat(T value);

        string ValueToString(T value);
    }
}