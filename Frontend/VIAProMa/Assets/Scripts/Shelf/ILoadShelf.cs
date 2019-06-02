using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadShelf
{
    void LoadContent();

    MessageBadge MessageBadge { get; }
}
