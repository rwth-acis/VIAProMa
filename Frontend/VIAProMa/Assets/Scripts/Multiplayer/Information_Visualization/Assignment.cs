using i5.ViaProMa.Visualizations.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class Assignment
{
    public int id;
    public string name;
    public float score;
    public string description;
    public string feedback;
}