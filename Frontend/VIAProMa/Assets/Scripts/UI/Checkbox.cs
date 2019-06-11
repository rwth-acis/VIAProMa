using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkbox : MonoBehaviour
{
    public event EventHandler OnValueChanged;

    public bool IsChecked { get; private set; }

    public void OnClick()
    {
        IsChecked = !IsChecked;
        OnValueChanged?.Invoke(this, EventArgs.Empty);
    }
}
