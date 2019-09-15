using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationPositionController : MonoBehaviour, IConfigurationController
{
    [SerializeField] private Vector3[] positionsForAvatars;

    public int AvatarIndex { get; set; }
    public int ModelIndex { get; set; }
    public int MaterialIndex { get; set; }
    public int ColorIndex { get; set; }

    public event EventHandler ConfigurationChanged;

    private void Awake()
    {
        if (positionsForAvatars.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(positionsForAvatars));
        }
    }

    public void ApplyConfiguration()
    {
        if (positionsForAvatars.Length == 0)
        {
            return;
        }

        int selectedIndex = AvatarIndex;
        if (selectedIndex >= positionsForAvatars.Length || selectedIndex < 0)
        {
            selectedIndex = 0;
        }
        transform.localPosition = positionsForAvatars[selectedIndex];
    }
}
