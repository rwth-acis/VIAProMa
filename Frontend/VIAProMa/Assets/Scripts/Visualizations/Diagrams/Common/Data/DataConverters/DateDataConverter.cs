using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public class DateDataConverter : IDataConverter<DateTime>
    {
        private DateTime minDate;

        public DateDataConverter(DateTime minDate)
        {
            this.minDate = minDate;
        }

        public DateTime FloatToValue(float f)
        {
            return minDate + new TimeSpan((int)f, 0, 0, 0);
        }

        public float ValueToFloat(DateTime value)
        {
            TimeSpan diff = value - minDate;
            return diff.Days;
        }

        public string ValueToString(DateTime value)
        {
            return value.ToShortDateString();
        }
    }
}