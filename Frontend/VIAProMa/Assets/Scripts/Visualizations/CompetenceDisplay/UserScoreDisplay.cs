using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScoreDisplay : DataDisplay<UserScore>
{
    [SerializeField] private GameObject scoreBarPrefab;

    private UserDataDisplay userDisplay;
    private List<GameObject> scoreBars;

    private void Awake()
    {
        userDisplay = GetComponentInChildren<UserDataDisplay>();
        if (scoreBarPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(scoreBarPrefab));
        }
        scoreBars = new List<GameObject>();
        for (int i=0;i<4;i++)
        {
            GameObject scoreBarInstance = Instantiate(scoreBarPrefab);
            scoreBarInstance.SetActive(false);
            scoreBars.Add(scoreBarInstance);
        }
    }

    public override void Setup(UserScore content)
    {
        base.Setup(content);
        userDisplay.Setup(content.User);
        foreach(GameObject scoreBar in scoreBars)
        {
            scoreBar.SetActive(true);
        }
    }

    public override void UpdateView()
    {
        base.UpdateView();
        userDisplay.UpdateView();

    }
}
