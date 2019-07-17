using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] protected MessageBadge messageBadge;
    [SerializeField] protected Interactable upButton;
    [SerializeField] protected Interactable downButton;

    protected int page;

    protected virtual void Awake()
    {
        if (messageBadge == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(messageBadge));
        }
        if (upButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(upButton));
        }
        if (downButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(downButton));
        }
    }

    protected virtual void CheckControls()
    {
        if (page <= 0)
        {
            upButton.Enabled = false;
        }
        else
        {
            upButton.Enabled = true;
        }
    }

    public void ResetPage()
    {
        page = 0;
    }

    public virtual void ScrollUp()
    {
        page--;
        page = Mathf.Max(0, page);
    }

    public virtual void ScrollDown()
    {
        page++;
    }
}
