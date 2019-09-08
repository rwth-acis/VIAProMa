using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
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