using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component which receives events from the fold/unfold animation of the main menu
/// The received animation events are translated to C# events so that other components can access them
/// </summary>
public class MainMenuAnimationEventHandler : MonoBehaviour
{
    /// <summary>
    /// Event which is raised once the main menu cube is fully folded
    /// The event is also raised if the cube starts unfolding
    /// </summary>
    public event EventHandler CubeFolded;

    /// <summary>
    /// Event which is raised once the main menu cube is fully unfolded
    /// The event is also raised if the cube starts folding
    /// </summary>
    public event EventHandler CubeUnfolded;

    /// <summary>
    /// Called in the end of the fold animation and the beginning of the unfold animation
    /// Raises the corresponding C# event
    /// </summary>
    /// <param name="animEvent">Data of the AnimationEvent</param>
    private void Folded(AnimationEvent animEvent)
    {
        CubeFolded?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called in the end of the unfold animation and the beginning of the fold animatoin
    /// Raises the corresponding C# event
    /// </summary>
    /// <param name="animEvent">Data of the AnimationEvent</param>
    private void Unfolded(AnimationEvent animEvent)
    {
        CubeUnfolded?.Invoke(this, EventArgs.Empty);
    }
}
