using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkParticipantDataDisplay : DataDisplay<NetworkParticipantData>
{
    [SerializeField] private TextMeshPro ParticipantNameLabel;

    private Interactable button;

    private void Awake()
    {
        if (ParticipantNameLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(ParticipantNameLabel));
        }
    }

    public override void Setup(NetworkParticipantData content) 
    {
        button = GetComponent<Interactable>();
        if (button == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Interactable), gameObject);
        }
        base.Setup(content);
    }

    public override void UpdateView()
    {
        base.UpdateView();
        int listLenght = content.Participants.Length;
        if (content != null && content.Participants != null)
        {
            //to do: change lable input 
            ParticipantNameLabel.text = "user";
        }
        else
        {
            ParticipantNameLabel.text = "NULL";
        }
    }

}
