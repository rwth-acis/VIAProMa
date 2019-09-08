using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
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
