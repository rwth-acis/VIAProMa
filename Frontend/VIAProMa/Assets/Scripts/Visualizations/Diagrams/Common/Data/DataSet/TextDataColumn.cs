using i5.VIAProMa.Visualizations.Common.Data.DataConverters;
using System.Collections.Generic;

namespace i5.VIAProMa.Visualizations.Common.Data.DataSets
{
    public class TextDataColumn : DataColumn<string>
    {
        public TextDataColumn(List<string> values) : base(values)
        {
        }

        protected override IDataConverter<string> DataConverter
        {
            get
            {
                return new StringDataConverter(Values);
            }
        }

        public override DataType DataType
        {
            get
            {
                return DataType.TEXT;
            }
        }
    }
}