using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] protected MessageBadge messageBadge;

    protected virtual void Awake()
    {
        if (messageBadge == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(messageBadge));
        }
    }
}
