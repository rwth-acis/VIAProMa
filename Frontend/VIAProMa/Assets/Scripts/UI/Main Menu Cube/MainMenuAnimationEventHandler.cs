using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnimationEventHandler : MonoBehaviour
{
    public event EventHandler CubeFolded;
    public event EventHandler CubeUnfolded;

    private void Folded(AnimationEvent animEvent)
    {
        CubeFolded?.Invoke(this, EventArgs.Empty);
    }

    private void Unfolded(AnimationEvent animEvent)
    {
        CubeUnfolded?.Invoke(this, EventArgs.Empty);
    }
}
