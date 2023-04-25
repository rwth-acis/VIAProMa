using UnityEngine;
using TMPro;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataDisplays;
using i5.VIAProMa.Login;

/// <summary>
/// Manages the active state of the delete button of issue cards
/// </summary>
public class DeleteButtonOpener : MonoBehaviour
{

    [SerializeField] private GameObject deleteButton;
    [SerializeField] private SourceDisplay sourceDisplay;

    private void Start()
    {
        sourceDisplay.OnContentSetup += UpdateButtonState;
        UpdateButtonState();
    }

    private void OnDestroy()
    {
        sourceDisplay.OnContentSetup -= UpdateButtonState;
    }

    /// <summary>
    /// Activate the delete button only if for requirements bazaar projects, GitHub does not support the deletion of issues
    /// </summary>
    private void UpdateButtonState()
    {
        deleteButton.SetActive(sourceDisplay.Content.Source == DataSource.REQUIREMENTS_BAZAAR);
    }
}