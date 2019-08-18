using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSelector : VariantSelector
{
    private List<AvatarPart> avatarParts;

    public List<AvatarPart> AvatarParts
    {
        get => avatarParts;
        set
        {
            avatarParts = value;
            UpdateDisplays();
        }
    }

    private void UpdateDisplays()
    {
        for (int i=0;i<itemFrames.Length;i++)
        {
            int index = (page * itemFrames.Length) + i;
            if (index < avatarParts.Count)
            {
                itemFrames[i].gameObject.SetActive(true);
                itemFrames[i].ItemIndex = (page * itemFrames.Length) + i;
                itemFrames[i].UpdateDisplay();
            }
            else
            {
                itemFrames[i].ItemIndex = 0;
                itemFrames[i].gameObject.SetActive(false);
            }
        }
    }

    public override void Select(int index)
    {
        // apply avatarParts[index]
    }
}
