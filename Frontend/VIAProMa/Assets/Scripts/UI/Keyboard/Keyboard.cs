using HoloToolkit.Unity;
using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the keyboard UI element
/// </summary>
public class Keyboard : Singleton<Keyboard>
{
    [Tooltip("Reference to the input field which shows the typed text")]
    [SerializeField] private InputField inputField;

    [Tooltip("Reference to the object which indicates where new text is inserted (typical text cursor)")]
    [SerializeField] private GameObject cursorObject;

    /// <summary>
    /// Length of the interval in after which the cursor's visibility is toggled
    /// This achieves the typical blinking effect of text cursors
    /// </summary>
    public float cursorBlinkTime = 0.53f;

    [Tooltip("References to key set pages between the user can switch in order to type standard letters and special characters")]
    [SerializeField] private GameObject[] keySetPages;

    [Tooltip("Grid for the autocomplete items")]
    [SerializeField] private GridObjectCollection autocompleteGrid;

    [Tooltip("Autocomplete item prefab")]
    [SerializeField] private GameObject autocompleteItemPrefab;

    [Tooltip("Maximum number of autocomplete items which should be shown")]
    public int maxNumberOfAutocompleteItems = 4;

    /// <summary>
    /// Event which is raised once the user has finished typing and dismisses the keyboard
    /// Contains information about the typed text and whether the input was aborted or accepted by the user
    /// </summary>
    public event InputFinishedEventHandler InputFinished;
    /// <summary>
    /// Event which is raised every time that the text is changed
    /// </summary>
    public event EventHandler TextChanged;

    private string text = "";
    private bool shiftActive;
    private bool capslockActive;

    private int cursorPos = 0;
    private bool cursorShowing;

    private IShiftableKey[] shiftableKeys;

    private int currentKeySetPageIndex;

    private List<string> autoCompleteOptions;
    private List<string> prioritisedMatches;
    private List<string> remainingMatches;
    private List<AutocompleteItem> autocompleteItems;

    /// <summary>
    /// The text on the keyboard
    /// </summary>
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
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// True if shift is enabled
    /// Updates all shiftable keys
    /// </summary>
    public bool ShiftActive
    {
        get { return shiftActive; }
        set
        {
            shiftActive = value;
            UpdateShiftKeys();
        }
    }

    /// <summary>
    /// True if capslock is enabled
    /// Overrules the value of ShiftActive
    /// Updates all shiftable keys
    /// </summary>
    public bool CapslockActive
    {
        get { return capslockActive; }
        set
        {
            capslockActive = value;
            UpdateShiftKeys();
        }
    }

    /// <summary>
    /// The position of the cursor in the text
    /// </summary>
    public int CursorPos
    {
        get { return cursorPos; }
        set
        {
            cursorPos = Mathf.Clamp(value, 0, text.Length); // make sure that the position of the cursor is still in bounds
            UpdateView();
        }
    }

    /// <summary>
    /// The index of the current key set page
    /// </summary>
    public int CurrentKeySetPageIndex
    {
        get => currentKeySetPageIndex;
        set
        {
            currentKeySetPageIndex = value;
            UpdateKeySetPageVisibility();
        }
    }

    /// <summary>
    /// Initializes the GameObject
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        shiftableKeys = GetComponentsInChildren<IShiftableKey>();

        if (inputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(inputField));
        }
        if (cursorObject == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(cursorObject));
        }
        if (keySetPages.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(keySetPages));
        }
        else
        {
            for (int i = 0; i < keySetPages.Length; i++)
            {
                if (keySetPages[i] == null)
                {
                    SpecialDebugMessages.LogArrayMissingReferenceError(this, nameof(keySetPages), i);
                }
            }
        }
        if (autocompleteGrid == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(autocompleteGrid));
        }
        if (autocompleteItemPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(autocompleteItemPrefab));
        }
        else
        {
            CreateAutocompleteItems();
        }

        autoCompleteOptions = new List<string>();
        prioritisedMatches = new List<string>();
        remainingMatches = new List<string>();

        // update the view to set the input field's text
        UpdateView();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // start the infinite coroutine which makes the cursor blink
        StartCoroutine(BlinkingCursor());
    }

    private void OnDisable()
    {

    }

    /// <summary>
    /// Displays the keyboard UI element at the given (global) position with the given (global) euler rotation
    /// The keyboard will be initialized with an empty text
    /// </summary>
    /// <param name="position">The global position where the keyboard should appear</param>
    /// <param name="eulerRotation">THe keyboard's global rotation in euler angles</param>
    public void Open(Vector3 position, Vector3 eulerRotation)
    {
        Open(position, eulerRotation, "");
    }

    /// <summary>
    /// Displays the keyboard UI element at the given (global) position with the given (global) euler rotation
    /// The keyboard will be set up with the given text
    /// </summary>
    /// <param name="position">The global position where the keyboard should appear</param>
    /// <param name="eulerRotation">THe keyboard's global rotation in euler angles</param>
    /// <param name="text">The text which will initially be shown on the keyboard</param>
    public void Open(Vector3 position, Vector3 eulerRotation, string text)
    {
        Open(position, eulerRotation, text, new List<string>());
    }

    public void Open(Vector3 position, Vector3 eulerRotation, List<string> autoCompleteOptions)
    {
        Open(position, eulerRotation, "", autoCompleteOptions);
    }

    public void Open(Vector3 position, Vector3 eulerRotation, string text, List<string> autoCompleteOptions)
    {
        // show keyboard
        gameObject.SetActive(true);
        // go back to the standard letters
        CurrentKeySetPageIndex = 0;

        // set up the keyboard according to the given parameters
        Text = text;
        transform.position = position;
        transform.eulerAngles = eulerRotation;

        CursorPos = text.Length;

        this.autoCompleteOptions = autoCompleteOptions;
        if (this.autoCompleteOptions == null)
        {
            this.autoCompleteOptions = new List<string>();
        }
        else
        {
            this.autoCompleteOptions.Sort();
        }
    }

    /// <summary>
    /// Backspace is applied to the keyboard
    /// Deletes the letter in front of the current cursor position
    /// </summary>
    public void Backspace()
    {
        if (cursorPos > 0)
        {
            // move the cursor one to the left
            CursorPos--;
            // remove the char which is now on the right of the cursor
            Text = Text.Remove(cursorPos, 1);
        }
    }

    /// <summary>
    /// Causes a delete operation
    /// Deletes the letter behind the current cursor position
    /// </summary>
    public void Delete()
    {
        if (cursorPos < Text.Length)
        {
            Text = text.Remove(cursorPos, 1);
        }
    }

    /// <summary>
    /// Inserts the given character behind the current cursor position
    /// </summary>
    /// <param name="character">The character to insert</param>
    public void AddLetter(char character)
    {
        if (cursorPos == text.Length)
        {
            Text += character;
        }
        else
        {
            Text = Text.Insert(cursorPos, character.ToString());
        }
        // also move the cursor forward so that we can keep typing
        CursorPos++;
        ShiftActive = false;
    }

    /// <summary>
    /// Finishes the input
    /// Should usually be called by the Unity event on the accept and abort keys
    /// </summary>
    /// <param name="aborted">Setting this to true indicates that the input was aborted (and not regularly finished)</param>
    public void InputDone(bool aborted)
    {
        // raise the input finished event
        InputFinishedEventArgs args = new InputFinishedEventArgs();
        args.Aborted = aborted;
        args.Text = Text;
        InputFinished?.Invoke(this, args);

        // deactivate the keyboard again
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Causes the input field to update the applied text
    /// </summary>
    private void UpdateView()
    {
        inputField.Text = text;
        // force a mesh update so that the text is already applied when the text cursor tries to find the correct position
        inputField.ContentField.ForceMeshUpdate(true);
        PositionCursor();

        if (autoCompleteOptions.Count > 0 && text.Length > 0)
        {
            FindAutoCompleteMatches();
            DisplayAutoCompleteMatches();
        }
        else
        {
            foreach (AutocompleteItem item in autocompleteItems)
            {
                item.Text = "";
            }
        }
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
                    Vector3.Lerp(inputField.ContentField.textInfo.characterInfo[cursorPos - 1].topRight,
                    inputField.ContentField.textInfo.characterInfo[cursorPos - 1].bottomRight, 0.5f)
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
        for (int i = 0; i < keySetPages.Length; i++)
        {
            if (i == currentKeySetPageIndex)
            {
                keySetPages[i].SetActive(true);
            }
            else
            {
                keySetPages[i].SetActive(false);
            }
        }
    }

    private void CreateAutocompleteItems()
    {
        autocompleteItems = new List<AutocompleteItem>();
        for (int i = 0; i < maxNumberOfAutocompleteItems; i++)
        {
            GameObject autocompleteItemInstance = Instantiate(autocompleteItemPrefab, autocompleteGrid.transform);
            AutocompleteItem item = autocompleteItemInstance.GetComponent<AutocompleteItem>();
            if (item == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AutocompleteItem), autocompleteItemInstance);
            }
            item.Setup(this);
            autocompleteItems.Add(item);
        }
    }

    private void FindAutoCompleteMatches()
    {
        prioritisedMatches.Clear();
        remainingMatches.Clear();
        foreach (string option in autoCompleteOptions)
        {
            string optionLower = option.ToLowerInvariant();
            string textLower = text.ToLowerInvariant();
            if (optionLower.StartsWith(textLower))
            {
                prioritisedMatches.Add(option);
            }
            else if (optionLower.Contains(textLower))
            {
                remainingMatches.Add(option);
            }
        }
    }

    private void DisplayAutoCompleteMatches()
    {
        int itemIndex = 0;
        // insert prioritised matches first
        for (int i = 0; i < prioritisedMatches.Count; i++)
        {
            if (itemIndex >= maxNumberOfAutocompleteItems)
            {
                break;
            }
            autocompleteItems[itemIndex].Text = prioritisedMatches[i];
            itemIndex++;
        }
        // insert remaining matches
        for (int i = 0; i < remainingMatches.Count; i++)
        {
            if (itemIndex >= maxNumberOfAutocompleteItems)
            {
                break;
            }
            autocompleteItems[itemIndex].Text = remainingMatches[i];
            itemIndex++;
        }
        // empty remaining items
        for (int i = itemIndex; i < maxNumberOfAutocompleteItems; i++)
        {
            autocompleteItems[i].Text = "";
        }
        // update the grid to show the items in the correct position
        autocompleteGrid.UpdateCollection();
        // position the grid
        Vector3 gridPos = new Vector3(0, 0.32f, 0);
        gridPos.y += (itemIndex * autocompleteGrid.CellHeight) / 2f + 0.01f;
        autocompleteGrid.transform.localPosition = gridPos;
    }
}