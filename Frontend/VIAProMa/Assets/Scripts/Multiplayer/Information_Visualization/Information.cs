using i5.ViaProMa.Visualizations.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class Information
{
    public string name;
    public string email;
    public float average_score;
    public Assignment[] assignments;
}