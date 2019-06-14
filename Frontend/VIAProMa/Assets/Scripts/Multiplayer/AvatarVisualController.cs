using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AvatarVisualController : MonoBehaviourPun
{
    [SerializeField] private TextMeshPro nameDisplay;
    [SerializeField] private GameObject avatarVisuals;

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

    private void Update()
    {
        nameDisplay.transform.position = transform.position + new Vector3(0, 0.3f, 0);
    }

    private void OnDestroy()
    {
        // remember to remove the name display separately since it is not part of the object anymore
        Destroy(nameDisplay.gameObject);
    }
}
