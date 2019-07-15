using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisualizationContentProvider
{
    List<Issue> Issues { get; }

    event EventHandler ContentChanged;
}
