using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class OpenMockUp : MonoBehaviourPunCallbacks
{
    private AssetBundle myLoadedAssetBundle;

    // Start is called before the first frame update
    void Start()
    {
        myLoadedAssetBundle = AssetBundle.LoadFromFile("Assets/Tests/MockUpEditor");
    }

    public void openNewScene() 
    {
        SceneManager.LoadScene("MockUpTest", LoadSceneMode.Single);
    }
}
