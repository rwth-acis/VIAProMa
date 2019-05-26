using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject cursorObject;

    public float cursorBlinkTime = 0.53f;

    private string text = "";
    private bool shiftActive;
    private bool capslockActive;

    [SerializeField] private int cursorPos = 0;
    private bool cursorShowing;

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
        set
        {
            shiftActive = value;
            UpdateShiftKeys();
        }
    }

    public bool CapslockActive
    {
        get { return capslockActive; }
        set
        {
            capslockActive = value;
            UpdateShiftKeys();
        }
    }

    public int CursorPos
    {
        get { return cursorPos; }
        set
        {
            cursorPos = value;
            UpdateView();
        }
    }

    private void Awake()
    {
        shiftableKeys = GetComponentsInChildren<IShiftableKey>();
        if (inputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(inputField));
        }
        StartCoroutine(BlinkingCursor());
    }

    public void Open(Vector3 position, Vector3 eulerRotation)
    {

    }

    public void Backspace()
    {
        if (cursorPos > 0)
        {
            // remove the char in front of the cursor
            Text = Text.Remove(cursorPos - 1);
            CursorPos--;
        }
    }

    public void AddLetter(char letter)
    {
        if (cursorPos == text.Length)
        {
            Text += letter;
        }
        else
        {
            Text.Insert(cursorPos, letter.ToString());
        }
        CursorPos++;
    }

    private void UpdateView()
    {
        inputField.Text = text;
        PositionCursor();
    }

    private void UpdateShiftKeys()
    {
        for (int i = 0; i < shiftableKeys.Length; i++)
        {
            shiftableKeys[i].SetShift(shiftActive || capslockActive);
        }
    }

    private IEnumerator BlinkingCursor()
    {
        while (true)
        {
            cursorObject.SetActive(cursorShowing);
            PositionCursor();
            cursorShowing = !cursorShowing;
            yield return new WaitForSeconds(cursorBlinkTime);
        }
    }

    private void PositionCursor()
    {
        if (cursorPos == text.Length && cursorPos != 0)
        {
            // position the cursor at the middle between the top and bottom of the current character
            cursorObject.transform.position = inputField.transform.TransformPoint(
                    Vector3.Lerp(inputField.ContentField.textInfo.characterInfo[cursorPos-1].topRight,
                    inputField.ContentField.textInfo.characterInfo[cursorPos-1].bottomRight, 0.5f)
                    )
                    + new Vector3(0, 0, -0.0051f); // also move it to the front so that it is not inside of the input field
        }
        else
        {
            // position the cursor at the middle between the top and bottom of the current character
            cursorObject.transform.position = inputField.transform.TransformPoint(
                    Vector3.Lerp(inputField.ContentField.textInfo.characterInfo[cursorPos].topLeft,
                    inputField.ContentField.textInfo.characterInfo[cursorPos].bottomLeft, 0.5f)
                    )
                    + new Vector3(0, 0, -0.0051f); // also move it to the front so that it is not inside of the input field
        }
    }
}