using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuidedTourStartWidget : MonoBehaviour
{

    public void LoadTutorial()
    {
        SceneManager.LoadScene("GuidedTourScene");
    }

    public void OnCancel()
    {
        PlayerPrefs.SetInt("GuidedTour", 1);
        Destroy(gameObject);
    }
}
