using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keyboard : Singleton<Keyboard>
{
    [SerializeField] private InputField inputField;

    [SerializeField] private GameObject cursorObject;
    public float cursorBlinkTime = 0.53f;

    [SerializeField] private GameObject[] keySetPages;

    public event InputFinishedEventHandler InputFinished;
    public event EventHandler TextChanged;

    private string text = "";
    private bool shiftActive;
    private bool capslockActive;

    private int cursorPos = 0;
    private bool cursorShowing;

    private IShiftableKey[] shiftableKeys;

    private int currentKeySetPageIndex;

    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            text = value;
            cursorPos = Mathf.Clamp(cursorPos, 0, text.Length); // make sure that the position of the cursor is still in bounds
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
            cursorPos = Mathf.Clamp(value, 0, text.Length); // make sure that the position of the cursor is still in bounds
            UpdateView();
        }
    }

    public int CurrentKeySetPageIndex
    {
        get => currentKeySetPageIndex;
        set
        {
            currentKeySetPageIndex = value;
            UpdateKeySetPageVisibility();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        shiftableKeys = GetComponentsInChildren<IShiftableKey>();
        if (inputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(inputField));
        }
        StartCoroutine(BlinkingCursor());
        UpdateView();
    }

    public void Open(Vector3 position, Vector3 eulerRotation)
    {
        Open(position, eulerRotation, "");
    }

    public void Open(Vector3 position, Vector3 eulerRotation, string text)
    {
        gameObject.SetActive(true);

        CurrentKeySetPageIndex = 0;

        Text = "";
        transform.position = position;
        transform.eulerAngles = eulerRotation;
    }

    public void Backspace()
    {
        if (cursorPos > 0)
        {
            // remove the char in front of the cursor
            Text = Text.Remove(cursorPos - 1, 1);
            CursorPos--;
            TextChanged?.Invoke(this, EventArgs.Empty);
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
            Text = Text.Insert(cursorPos, letter.ToString());
        }
        CursorPos++;
        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    public void InputDone(bool aborted)
    {
        InputFinishedEventArgs args = new InputFinishedEventArgs();
        args.Aborted = aborted;
        args.Text = Text;
        InputFinished?.Invoke(this, args);

        gameObject.SetActive(false);
    }

    private void UpdateView()
    {
        inputField.Text = text;
        inputField.ContentField.ForceMeshUpdate(true);
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

    private void UpdateKeySetPageVisibility()
    {
        for (int i=0;i<keySetPages.Length;i++)
        {
            if (i== currentKeySetPageIndex)
            {
                keySetPages[i].SetActive(true);
            }
            else
            {
                keySetPages[i].SetActive(false);
            }
        }
    }
}