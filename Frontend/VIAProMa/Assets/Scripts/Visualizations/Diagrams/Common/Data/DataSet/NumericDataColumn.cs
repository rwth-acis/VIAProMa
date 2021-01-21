using i5.VIAProMa.Visualizations.Common.Data.DataConverters;
using System.Collections.Generic;

namespace i5.VIAProMa.Visualizations.Common.Data.DataSets
{
    public class NumericDataColumn : DataColumn<float>
    {
        public NumericDataColumn(List<float> values) : base(values)
        {
        }

        protected override IDataConverter<float> DataConverter
        {
            get
            {
                return new FloatDataConverter();
            }
        }

        public override DataType DataType
        {
            get
            {
                return DataType.NUMERIC;
            }
        }
    }
}
