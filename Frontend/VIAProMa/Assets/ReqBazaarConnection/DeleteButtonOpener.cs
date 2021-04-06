using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;

public class DeleteButtonOpener : MonoBehaviour
{

    [SerializeField] private GameObject deleteButtonPrefab;
    [SerializeField] private TextMeshPro requirementName;

    private GameObject buttonInstance;

    // Start is called before the first frame update
    private void Start()
    {
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginCompleted;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LogoutCompleted;
    }

    private void Update()
    {
        if(buttonInstance == null)
        {
            buttonInstance = Instantiate(deleteButtonPrefab, new Vector3(this.transform.position.x + 0.08f, this.transform.position.y + 0.1f, this.transform.position.z), Quaternion.identity);
            buttonInstance.GetComponent<DeleteButton>().requirementName = requirementName;
            buttonInstance.SetActive(ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).IsLoggedIn);
        }
        if(buttonInstance.transform.position.x > this.transform.position.x + 0.08f || buttonInstance.transform.position.y > this.transform.position.x + 0.1f)
        {
            buttonInstance.transform.position = new Vector3(this.transform.position.x + 0.08f, this.transform.position.y + 0.1f, this.transform.position.z);
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


    public void OnDestroy()
    {
        Destroy(buttonInstance);
    }
}
