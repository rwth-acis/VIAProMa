using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterKey : Key, IShiftableKey
{
    [SerializeField] private TextMeshPro letterCaption;

    public char letter;

    private bool shiftActive;

    protected override void Awake()
    {
        base.Awake();
        if (letterCaption == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(letterCaption));
        }
    }

    public void SetShift(bool shiftActive)
    {
        this.shiftActive = shiftActive;
        UpdateView();
    }

    protected override void KeyPressed()
    {
        base.KeyPressed();
        char applyToText = char.ToLower(letter);
        if (shiftActive)
        {
            applyToText = char.ToUpper(letter);
        }
        keyboard.AddLetter(applyToText);
    }

    private void UpdateView()
    {
        if (letterCaption != null)
        {
            letterCaption.text = shiftActive ? char.ToUpper(letter).ToString() : letter.ToString();
        }
    }

    private void OnValidate()
    {
        UpdateView();
    }
}
