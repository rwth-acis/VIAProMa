using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorItem : IItem
{
    public ColorItem(Color color)
    {
        DisplayColor = color;
    }

    public Sprite Sprite
    {
        get => null;
    }

    public Color DisplayColor
    {
        get;private set;
    }
}
