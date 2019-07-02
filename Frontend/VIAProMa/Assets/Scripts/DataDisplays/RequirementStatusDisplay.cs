using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RequirementStatusDisplay : DataDisplay<Requirement>
{
    [SerializeField] private TextMeshPro statusLabel;
    [SerializeField] private Renderer statusLabelBackground;

    private Contributors contributors;

    private void Awake()
    {
        if (statusLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(statusLabel));
        }
        if (statusLabelBackground == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(statusLabelBackground));
        }
    }

    public override async void Setup(Requirement requirement)
    {
        content = requirement;
        // also load the contributors
        ApiResult<Contributors> res = await  RequirementsBazaar.GetRequirementContributors(content.id);
        if (res.HasError)
        {
            statusLabel.text = "Error";
            statusLabelBackground.material.color = new Color(134f / 255f, 4f / 255f, 127f / 255f); // purple
        }
        else
        {
            contributors = res.Value;
            UpdateView();
        }
    }

    public override void UpdateView()
    {
        base.UpdateView();
        if (content == null || contributors == null)
        {
            return;
        }

        CardStatus status = DetermineStatus();

        if (status == CardStatus.OPEN)
        {
            statusLabel.text = "Open";
            statusLabelBackground.material.color = new Color(7f/255f, 187f/255f, 40f/255f); // green
        }
        else if (status == CardStatus.DEVELOPMENT)
        {
            statusLabel.text = "Development";
            statusLabelBackground.material.color = new Color(245f/255f, 233f/255f, 6f/255f); // yellow
        }
        else if (status == CardStatus.CLOSED)
        {
            statusLabel.text = "Closed";
            statusLabelBackground.material.color = new Color(181f / 255f, 25f / 255f, 25f / 255); // red
        }

    }

    private CardStatus DetermineStatus()
    {
        if (content == null || contributors == null)
        {
            return CardStatus.OPEN;
        }

        // if the card is realized: it is closed
        if (content.IsRealized)
        {
            return CardStatus.CLOSED;
        }

        // if there are no develoeprs for the requirement, it is not developed
        if ((contributors.developers == null || contributors.developers.Length == 0) && contributors.leadDeveloper.IsUninitialized)
        {
            return CardStatus.OPEN;
        }
        else
        {
            return CardStatus.DEVELOPMENT;
        }
    }
}

public enum CardStatus
{
    OPEN, DEVELOPMENT, CLOSED
}
