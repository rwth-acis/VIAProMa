using System;
using System.Collections.Generic;
using UnityEngine;

namespace DIG.GBLXAPI.Internal
{
    public class DurationSlotTracker
    {
        // Time tracking for result durations
        private List<DateTime> dateTimeSlots = new List<DateTime>();

		public float GetSlot(int index)
        {
			if (index < dateTimeSlots.Count && index >= 0)
			{
				// Returns difference in seconds
				TimeSpan ts = DateTime.Now - dateTimeSlots[index];
				return (float)ts.TotalSeconds; // Fractional
			}

			return -1f;
		}

		// ------------------------------------------------------------------------
		// ------------------------------------------------------------------------
		public void ResetSlot(int index)
		{
			if(index < 0) { return; }

			// Add slots up to requested slot number
			while (index >= dateTimeSlots.Count)
			{
				dateTimeSlots.Add(DateTime.Now);
			}

			if (index >= dateTimeSlots.Count)
			{
				Debug.LogWarning("ResetDurationSlot: Requested slot " + index + " less than " + dateTimeSlots.Count + " available slots");
			}

			// Set that slot to now
			dateTimeSlots[index] = DateTime.Now;
		}

		public void ResetAllSlots()
        {
			int slotCount = dateTimeSlots.Count;
            dateTimeSlots.Clear();
			ResetSlot(slotCount - 1);
        }
    }
}
