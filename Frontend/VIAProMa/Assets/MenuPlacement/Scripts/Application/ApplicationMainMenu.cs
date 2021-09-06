using i5.VIAProMa.UI.MainMenuCube;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using MenuPlacement;
public class ApplicationMainMenu : MenuBase
{
    
    [Header("UI Elements")]
    [SerializeField] private Interactable startButton;
    [SerializeField] private Interactable createButton;
    [SerializeField] private GameObject notification;

    [Header("Objects")]
    [SerializeField] private GameObject cubeToCreate;

    private MPFoldController foldContorller;

    // Start is called before the first frame update
    void Awake()
    {
        if (startButton == null) {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(startButton));
        }

        if (createButton == null) {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(createButton));
        }

        if (cubeToCreate == null) {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(cubeToCreate));
        }
        /*else {
            float cubeSizeOffset = 0.3f;
            cubeToCreate.transform.localScale = new Vector3(cubeToCreate.transform.localScale.x * cubeSizeOffset, cubeToCreate.transform.localScale.y * cubeSizeOffset, cubeToCreate.transform.localScale.y * cubeSizeOffset);
        }*/

        foldContorller = gameObject.GetComponent<MPFoldController>();
    }

    public void CreateCube() {
        GameObject createdCube = Instantiate(cubeToCreate, new Vector3(createButton.transform.position.x - 0.3f, createButton.transform.position.y, createButton.transform.position.z), Quaternion.Euler(0, 0, 0));
        createdCube.SetActive(true);
        foldContorller.FoldCube();
    }

    public void CloseNotification() {
        notification.SetActive(false);
    }

    public override void Initialize() {
        
    }

    public override void OnClose() {
        
    }
}
