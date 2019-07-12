using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the visual aspects of the avatar
/// Among others, it deactivates the own avatar and controls the name diplays
/// </summary>
public class AvatarVisualController : MonoBehaviourPun
{
    [SerializeField] private TextMeshPro nameDisplay;
    [SerializeField] private GameObject avatarVisuals;

    /// <summary>
    /// Checks if the component is set up correctly
    /// </summary>
    private void Awake()
    {
        if (nameDisplay == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(nameDisplay));
        }
        if (avatarVisuals == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(avatarVisuals));
        }
    }

    /// <summary>
    /// Initializes the component
    /// Deactivates the avatar's visual representation if it is the own avatar
    /// If the avatar represents a remote player, it displays the player's name
    /// </summary>
    private void Start()
    {
        if (photonView.IsMine)
        {
            avatarVisuals.SetActive(false);
            nameDisplay.gameObject.SetActive(false);
        }
        else
        {
            nameDisplay.text = photonView.Owner.NickName;
            nameDisplay.transform.parent = null; // remove the name label from the tag so that it does not get rotated
            // instead the name display will be updated every frame
        }
    }

    /// <summary>
    /// Keeps the name display above the avatar
    /// This is done in code and not by parenting so that the text is not affected by head rotations
    /// </summary>
    private void Update()
    {
        nameDisplay.transform.position = transform.position + new Vector3(0, 0.3f, 0);
    }

    /// <summary>
    /// Since the name display is not parented to the object, we need to destroy it if the avatar is destroyed
    /// </summary>
    private void OnDestroy()
    {
        // remember to remove the name display separately since it is not part of the object anymore
        Destroy(nameDisplay.gameObject);
    }
}
