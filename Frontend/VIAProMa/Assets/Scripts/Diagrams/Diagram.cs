using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Diagram : MonoBehaviour {

    protected Bounds pointBounds; // field is required because otherwise Bounds.Encapsulate does not work as intended

    public event EventHandler DiagramUpdated;

    public Bounds PointBounds
    {
        get
        {
            return pointBounds;
        }
    }

    protected virtual void UpdateVisuals()
    {
        OnDiagramUpdated(EventArgs.Empty);
    }

    protected virtual void OnDiagramUpdated(EventArgs e)
    {
        DiagramUpdated?.Invoke(this, e);
    }
}
