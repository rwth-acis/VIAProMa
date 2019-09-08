using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public class DateDataColumn : DataColumn<DateTime>
    {
        private DateTime minDate;

        public DateDataColumn(List<DateTime> values) : base(values)
        {
            if (values.Count > 0)
            {
                minDate = values[0];
                for (int i = 1; i < values.Count; i++)
                {
                    if (values[i] < minDate)
                    {
                        minDate = values[i];
                    }
                }
            }
            else
            {
                Debug.LogError("The date data column has no values");
            }
        }

        protected override IDataConverter<DateTime> DataConverter
        {
            get
            {
                return new DateDataConverter(minDate);
            }
        }
        public override DataType DataType
        {
            get
            {
                return DataType.DATE;
            }
        }
    }
}
