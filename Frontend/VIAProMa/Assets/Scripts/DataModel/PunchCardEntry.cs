using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PunchCardEntry
{
    public int day;
    public int hour;
    public int numberOfCommits;

    public DayOfWeek DayOfWeek
    {
        get { return (DayOfWeek)day; }
    }
}
