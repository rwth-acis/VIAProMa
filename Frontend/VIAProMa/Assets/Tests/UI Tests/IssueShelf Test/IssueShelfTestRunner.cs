using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueShelfTestRunner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (IssueSelectionManager.Instance.SelectionModeActive)
            {
                Debug.Log("Selection mode stopped");
                Debug.Log("Selected " + IssueSelectionManager.Instance.EndSelectionMode().Count + " issues");
            }
            else
            {
                Debug.Log("Selection mode active");
                IssueSelectionManager.Instance.StartSelectionMode();
            }
        }
    }
}
