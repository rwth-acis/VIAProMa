using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using TMPro;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataDisplays;
using i5.VIAProMa.Login;

public class EditButtonOpener : MonoBehaviour
{

    [SerializeField] private GameObject editButtonPrefab;
    [SerializeField] private TextMeshPro issueName;
    [SerializeField] private TextMeshPro issueDescription;
    [SerializeField] private SourceDisplay source;

    [HideInInspector] public int issueID;
    private GameObject buttonInstance;
    private DataSource dataSource;

    // Subscribe to Login and Logout Events
    private void Start()
    {
        ServiceManager.GetService<LearningLayersOidcService>().LoginCompleted += LoginCompleted_LearningLayers;
        ServiceManager.GetService<LearningLayersOidcService>().LogoutCompleted += LogoutCompleted_LearningLayers;
        ServiceManager.GetService<GitHubOidcService>().LoginCompleted += LoginCompleted_GitHub;
        ServiceManager.GetService<GitHubOidcService>().LogoutCompleted += LogoutCompleted_GitHub;
        dataSource = source.Content.Source;
        issueID = source.Content.Id;

    }

    private void Update()
    {
        if (buttonInstance == null)
        {
            //Instantiate Button next to the Issue Card and pass on the issue card name and description, the button is activated if the user is logged in
            if (dataSource == DataSource.REQUIREMENTS_BAZAAR)
            {
                buttonInstance = Instantiate(editButtonPrefab, new Vector3(this.transform.position.x + 0.02f, this.transform.position.y + 0.1f, this.transform.position.z), Quaternion.identity);
                buttonInstance.GetComponent<EditButton>().issueName = issueName;
                buttonInstance.GetComponent<EditButton>().issueDescription = issueDescription;
                buttonInstance.GetComponent<EditButton>().issueID = issueID;
                buttonInstance.SetActive(ServiceManager.GetService<LearningLayersOidcService>().IsLoggedIn);
                buttonInstance.GetComponent<EditButton>().source = DataSource.REQUIREMENTS_BAZAAR;
                buttonInstance.transform.parent = this.transform.parent.parent.parent;
            }
            else if (dataSource == DataSource.GITHUB)
            {
                buttonInstance = Instantiate(editButtonPrefab, new Vector3(this.transform.position.x + 0.08f, this.transform.position.y + 0.1f, this.transform.position.z), Quaternion.identity);
                buttonInstance.GetComponent<EditButton>().issueName = issueName;
                buttonInstance.GetComponent<EditButton>().issueDescription = issueDescription;
                buttonInstance.GetComponent<EditButton>().issueID = issueID;
                buttonInstance.SetActive(ServiceManager.GetService<GitHubOidcService>().IsLoggedIn);
                buttonInstance.GetComponent<EditButton>().source = DataSource.GITHUB;
                buttonInstance.transform.parent = this.transform.parent.parent.parent;
            }
        }
        if (dataSource == DataSource.REQUIREMENTS_BAZAAR && buttonInstance.transform.position != new Vector3(this.transform.position.x + 0.02f, this.transform.position.y + 0.1f, this.transform.position.z))
        {
            buttonInstance.transform.position = new Vector3(this.transform.position.x + 0.02f, this.transform.position.y + 0.1f, this.transform.position.z);
        }
        else if(dataSource == DataSource.GITHUB && buttonInstance.transform.position != new Vector3(this.transform.position.x + 0.08f, this.transform.position.y + 0.1f, this.transform.position.z))
        {
            buttonInstance.transform.position = new Vector3(this.transform.position.x + 0.08f, this.transform.position.y + 0.1f, this.transform.position.z);
        }
    }

    /// <summary>
    /// Instantiate delete button when logged in
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void LoginCompleted_LearningLayers(object sender, System.EventArgs e)
    {
        if (buttonInstance != null && dataSource == DataSource.REQUIREMENTS_BAZAAR)
            buttonInstance.SetActive(true);
    }

    /// <summary>
    /// Destroy delete button when logged out
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public void LogoutCompleted_LearningLayers(object sender, System.EventArgs e)
    {
        if (buttonInstance != null && dataSource == DataSource.REQUIREMENTS_BAZAAR)
            buttonInstance.SetActive(false);
    }

    /// <summary>
    /// Instantiate delete button when logged in
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void LoginCompleted_GitHub(object sender, System.EventArgs e)
    {
        if (buttonInstance != null && dataSource == DataSource.GITHUB)
            buttonInstance.SetActive(true);
    }

    /// <summary>
    /// Destroy delete button when logged out
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public void LogoutCompleted_GitHub(object sender, System.EventArgs e)
    {
        if (buttonInstance != null && dataSource == DataSource.GITHUB)
            buttonInstance.SetActive(false);
    }

    //Destroy button along with this object
    public void OnDestroy()
    {
        if (buttonInstance != null)
            Destroy(buttonInstance);
    }
}
