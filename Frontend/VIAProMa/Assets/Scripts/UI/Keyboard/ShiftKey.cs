using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftKey : Key, IShiftableKey
{
    [SerializeField] private GameObject shiftIndicator;

    protected override void Awake()
    {
        base.Awake();
        if (shiftIndicator == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(shiftIndicator));
        }
    }

    protected override void KeyPressed()
    {
        base.KeyPressed();
        keyboard.ShiftActive = !keyboard.ShiftActive;
    }

    public void SetShift(bool shiftActive)
    {
        shiftIndicator.SetActive(shiftActive);
    }
}
