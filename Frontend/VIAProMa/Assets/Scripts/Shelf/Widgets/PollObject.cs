using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.Multiplayer.Poll;
using TMPro;
using UnityEngine;

public class PollObject : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshPro question;
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
            question.text = PollHandler.Instance.savedPolls[pollIndex].Question;
        }
    }

    public void SelectPoll()
    {
        PollHandler.Instance.DisplayPollAtIndex(pollIndex);
    }
}
