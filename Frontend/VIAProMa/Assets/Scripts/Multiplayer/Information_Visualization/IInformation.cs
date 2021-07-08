using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class IInformation
{
    public PersonInfo _id;
    public float average_score;
    //public Assignment[] assignments;
    public float[] assignments;
}