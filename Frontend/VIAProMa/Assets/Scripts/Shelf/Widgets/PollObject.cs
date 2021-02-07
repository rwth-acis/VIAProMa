﻿using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.Multiplayer.Poll;
using TMPro;
using UnityEngine;

public class PollObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro question;
    private int pollIndex;

    public int PollIndex
    {
        get 
        {
            return pollIndex;
        }
        set
        {
            pollIndex = value;
            question.text = PollHandler.Instance.savedPolls[pollIndex]?.Question ?? "";
        }
    }

    public void SelectPoll()
    {
        PollHandler.Instance.GenerateSynchronizedPollDisplay(pollIndex+1);
    }
}
