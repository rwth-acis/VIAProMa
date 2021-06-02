using UnityEngine;
using TMPro;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataDisplays;
using i5.VIAProMa.UI.ListView.Issues;
using i5.Toolkit.Core.Utilities;

public class DeleteButtonOpener : MonoBehaviour
{

    [SerializeField] private GameObject deleteButtonPrefab;
    [SerializeField] private SourceDisplay source;

    [HideInInspector] public int requirementID;
    private GameObject buttonInstance;
    private DataSource dataSource;

    // Subscribe to Login and Logout Events
    private void Start()
    {
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginCompleted;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LogoutCompleted;
        dataSource = source.Content.Source;
        requirementID = source.Content.Id;
    }

    private void Update()
    {
        if(buttonInstance == null && dataSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            //Instantiate Button next to the Issue Card and pass on the requirement name, the button is activated if the user is logged in
            buttonInstance = Instantiate(deleteButtonPrefab, new Vector3(this.transform.position.x + 0.08f, this.transform.position.y + 0.1f, this.transform.position.z), Quaternion.identity);
            buttonInstance.GetComponent<DeleteButton>().requirementID = requirementID;
            buttonInstance.SetActive(ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).IsLoggedIn);
        }
        //Check if the placement of the button is indeed correct and next to the position
        if(buttonInstance != null)
        {
            if (buttonInstance.transform.position.x > this.transform.position.x + 0.08f || buttonInstance.transform.position.y > this.transform.position.y + 0.1f)
            {
                buttonInstance.transform.position = new Vector3(this.transform.position.x + 0.08f, this.transform.position.y + 0.1f, this.transform.position.z);
            }
        }
    }

    /// <summary>
    /// Instantiate delete button when logged in
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void LoginCompleted(object sender, System.EventArgs e)
    {
        if(buttonInstance != null)
            buttonInstance.SetActive(true);
    }

    /// <summary>
    /// Destroy delete button when logged out
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public void LogoutCompleted(object sender, System.EventArgs e)
    {
        if (buttonInstance != null)
            buttonInstance.SetActive(false);
    }

    //Destroy button along with this object
    public void OnDestroy()
    {
        Destroy(buttonInstance);
    }
}
