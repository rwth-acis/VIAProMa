using UnityEngine;
using TMPro;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataDisplays;
using i5.VIAProMa.Login;

public class DeleteButtonOpener : MonoBehaviour
{

    [SerializeField] private GameObject deleteButtonPrefab;
    [SerializeField] private SourceDisplay source;

    private DataSource dataSource;

    // Subscribe to Login and Logout Events
    private void Start()
    {
        dataSource = source.Content.Source;
    }

    private void Update()
    {
        deleteButtonPrefab.SetActive(dataSource == DataSource.REQUIREMENTS_BAZAAR);

    }
}