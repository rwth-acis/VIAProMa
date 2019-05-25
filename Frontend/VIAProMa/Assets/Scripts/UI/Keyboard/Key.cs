using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Key : MonoBehaviour
{
    protected string value;

    protected Keyboard keyboard;

    protected virtual void Awake()
    {
        Interactable interactable = GetComponent<Interactable>();
        interactable.OnClick.AddListener(KeyPressed);

        keyboard = transform.parent.GetComponent<Keyboard>();
        if (keyboard == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Keyboard), transform.parent.gameObject);
        }
    }

    protected virtual void KeyPressed()
    {
    }
}
