using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using i5.VIAProMa.Visualizations.ProgressBars;

[RequireComponent(typeof(LearningLocker))]
public class ProgressVisualizer : MonoBehaviour
{
    //public float percentDone = 0f;
    //public float percentInProgress = 0f;
    public GameObject progressBar;
    private IProgressBarVisuals progressBarVisuals;
    public string name {get; set;} = "";
    public TextAsset jsonFile;
    public event EventHandler ConfigurationChanged;
    private GameObject ProgressList;
    private LearningLocker locker;

    private void Awake()
    {
        name = PhotonNetwork.NickName;
        locker = GetComponent<LearningLocker>();
        progressBarVisuals = progressBar.GetComponent<IProgressBarVisuals>();
    }

    private void Start()
    {
        ProgressList = GameObject.Find("ProgressList");
        progressBar.transform.parent = ProgressList.transform;
    }

    public async void UpdateView()
    {
        //jsonFile = (TextAsset)Resources.Load(name, typeof(TextAsset));
        //Information MentorData = JsonUtility.FromJson<Information>(jsonFile.text);
        IInformation MentorData = await locker.GetInformation(name);

        if (jsonFile != null)
        {
            //float percentDone = MentorData.average_score/100f;
            float percentDone = MentorData.average_score;
            float percentInProgress = 1f - percentDone;

            progressBarVisuals.Title = name + ", average score";
            progressBarVisuals.PercentageDone = percentDone;
            progressBarVisuals.PercentageInProgress = percentInProgress;
        }
    }

}
