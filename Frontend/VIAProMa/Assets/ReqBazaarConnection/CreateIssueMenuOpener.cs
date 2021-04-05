using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using Microsoft.MixedReality.Toolkit.UI;

public class CreateIssueMenuOpener : MonoBehaviour
{
    [SerializeField] CreateIssueMenu createIssueMenu;
    bool isOpen = false;


    public void CloseMenu()
    {
        createIssueMenu.gameObject.SetActive(false);
        isOpen = false;
        Debug.Log("Try to close menu");
    }

    public void OpenMenu()
    {
        createIssueMenu.gameObject.SetActive(true);
        isOpen = true;
        Debug.Log("Try to open menu");
    }

    public void OpenCreateIssueMenu()
    {
        if(createIssueMenu != null)
        {
            if (isOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }
}
