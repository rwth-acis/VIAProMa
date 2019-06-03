using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequirementsLoader : Shelf, ILoadShelf
{
    [SerializeField] private ShelfConfiguration configuration;

    public MessageBadge MessageBadge { get => messageBadge; }


    public void LoadContent()
    {
        // load requirements from the correct project or category
    }
}
