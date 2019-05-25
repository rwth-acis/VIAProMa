using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    [SerializeField] private TextMeshPro inputField;

    private string text;
    private bool shiftActive;
    private bool capslockActive;

    private IShiftableKey[] shiftableKeys;

    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            text = value;
            UpdateView();
        }
    }

    public bool ShiftActive
    {
        get { return shiftActive; }
        set { shiftActive = value; }
    }

    public bool CapslockActive
    {
        get { return capslockActive; }
        set { capslockActive = value; }
    }

    private void Awake()
    {
        shiftableKeys = GetComponentsInChildren<IShiftableKey>();
    }

    public void Open()
    {

    }

    private void UpdateView()
    {
        inputField.text = text;
    }

    private void SetShiftKeys(bool value)
    {
        for (int i=0;i<shiftableKeys.Length;i++)
        {
            shiftableKeys[i].SetShift(value);
        }
    }
}
