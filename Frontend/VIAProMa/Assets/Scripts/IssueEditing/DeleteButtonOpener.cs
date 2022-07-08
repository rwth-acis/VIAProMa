using UnityEngine;
using TMPro;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataDisplays;
using i5.VIAProMa.Login;

public class DeleteButtonOpener : MonoBehaviour
{

    [SerializeField] private GameObject deleteButton;
    [SerializeField] private SourceDisplay sourceDisplay;

    // Subscribe to Login and Logout Events
    private void Start()
    {
        sourceDisplay.OnContentSetup += UpdateButtonState;
        UpdateButtonState();
    }

    private void OnDestroy()
    {
        sourceDisplay.OnContentSetup -= UpdateButtonState;
    }

    private void UpdateButtonState()
    {
        deleteButton.SetActive(sourceDisplay.Content.Source == DataSource.REQUIREMENTS_BAZAAR);
    }
}